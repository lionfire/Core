using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.Referencing;
using LionFire.Structures;
using Xunit;
using Microsoft.Extensions.Hosting;
using LionFire.DependencyInjection;
using System;

namespace LionUri_
{
    public class _GetScheme
    {
        #region Normal

        [Fact]
        public void Pass_GetScheme()
        {
            var result = LionUri.GetUriScheme("my-uri-scheme://myhost:1234/my/path?asdf");
            Assert.Equal("my-uri-scheme", result);
        }

        [Fact]
        public void Pass_TryGetScheme()
        {
            var result = LionUri.TryGetUriScheme("my-uri-scheme://myhost:1234/my/path?asdf");
            Assert.Equal("my-uri-scheme", result);
        }

        #endregion

        #region Typo

        [Fact]
        public void Fail_GetScheme_Typo()
        {
            Assert.Throws<ArgumentException>(() => LionUri.GetUriScheme("my-uri-scheme-missing//myhost/my/path?asdf"));
        }
        [Fact]
        public void Fail_TryGetScheme_Typo()
        {
            Assert.Null(LionUri.TryGetUriScheme("my-uri-scheme-missing//myhost/my/path?asdf"));
        }

        [Fact]
        public void Pass_GetScheme_Typo_SkipValidate()
        {
            Assert.Throws<ArgumentException>(() => LionUri.GetUriScheme("my-uri-scheme-missing//myhost/my/path?asdf", validateScheme: false));
        }
        [Fact]
        public void Pass_TryGetScheme_Typo_SkipValidate()
        {
            Assert.Null(LionUri.TryGetUriScheme("my-uri-scheme-missing//myhost/my/path?asdf", validateScheme: false));
        }

        #endregion

        #region Invalid Characters

        [Fact]
        public void Fail_GetScheme_Invalid()
        {
            Assert.Throws<ArgumentException>(() => LionUri.GetUriScheme("my-$uri-scheme://myhost/my/path?asdf"));
        }
        [Fact]
        public void Fail_TryGetScheme_Invalid()
        {
            Assert.Null(LionUri.TryGetUriScheme("my-$uri-scheme://myhost/my/path?asdf"));
        }

        
        [Fact]
        public void Pass_GetScheme_Invalid_SkipValidate()
        {
            Assert.Equal("my-$uri-scheme", LionUri.GetUriScheme("my-$uri-scheme://myhost/my/path?asdf", validateScheme: false));
        }
        [Fact]
        public void Pass_TryGetScheme_Invalid_SkipValidate()
        {
            Assert.Equal("my-$uri-scheme", LionUri.TryGetUriScheme("my-$uri-scheme://myhost/my/path?asdf", validateScheme: false));
        }

        #endregion

    }
}
