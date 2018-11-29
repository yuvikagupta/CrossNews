using System;
using System.Threading.Tasks;
using CrossNews.Core.Extensions;
using CrossNews.Core.Services;
using CrossNews.Core.ViewModels;
using Moq;
using MvvmCross.Navigation;
using Xunit;

namespace CrossNews.Core.Tests.ViewModels
{
    public class SettingsViewModelTests : ViewModelFixtureBase
    {
        private Mock<IMvxNavigationService> Navigation => new Mock<IMvxNavigationService>();
        private Mock<IBrowserService> Browser => new Mock<IBrowserService>();
        private Mock<IAppService> App => new Mock<IAppService>().SetupAllProperties();

        [Fact]
        public void VersionStringContainsAppName()
        {
            var app = App;
            app.SetupGet(a => a.Name).Returns("APPNAME");

            var sut = new SettingsViewModel(Navigation.Object, Browser.Object, app.Object);

            Assert.Contains("APPNAME", sut.VersionString);
        }

        [Fact]
        public void VersionStringContainsPlatformName()
        {
            var app = App;
            app.SetupGet(a => a.Platform).Returns("PLATFORM");

            var sut = new SettingsViewModel(Navigation.Object, Browser.Object, app.Object);

            Assert.Contains("PLATFORM", sut.VersionString);
        }

        [Fact]
        public void VersionStringContainsAppVersion()
        {
            var app = App;
            app.SetupGet(a => a.Version).Returns("APPVERSION");

            var sut = new SettingsViewModel(Navigation.Object, Browser.Object, app.Object);

            Assert.Contains("APPVERSION", sut.VersionString);
        }

        [Fact]
        public void VersionStringContainsAppBuildNumber()
        {
            var app = App;
            app.SetupGet(a => a.BuildNumber).Returns(99);

            var sut = new SettingsViewModel(Navigation.Object, Browser.Object, app.Object);

            Assert.Contains("99", sut.VersionString);
        }

        [Fact]
        public void ShowProjectSiteCommandLaunchesSiteOnInternalBrowser()
        {
            var browser = Browser;
            Uri calledUri = null;
            browser
                .Setup(b => b.ShowInBrowserAsync(It.IsAny<Uri>(), true))
                .ReturnsAsync(true)
                .Callback((Uri u, bool i) => calledUri = u)
                .Verifiable();

            var sut = new SettingsViewModel(Navigation.Object, browser.Object, App.Object);
            sut.ShowProjectSiteCommand.TryExecute();

            browser.Verify(b => b.ShowInBrowserAsync(It.IsAny<Uri>(), true), Times.Once);
            var expectedUri = new Uri("https://github.com/kipters/CrossNews");
            Assert.Equal(expectedUri, calledUri);
        }

        [Fact]
        public void ShowLicensesCommandNavigatesToLicensesViewModel()
        {
            var navigation = Navigation;
            navigation
                .Setup(n => n.Navigate<LicensesViewModel>(null, default))
                .ReturnsAsync(true)
                .Verifiable();

            var sut = new SettingsViewModel(navigation.Object, Browser.Object, App.Object);
            sut.ShowLicensesCommand.TryExecute();

            navigation.Verify();
        }
    }
}