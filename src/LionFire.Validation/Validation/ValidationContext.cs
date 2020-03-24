using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{

    //public interface IValidation
    //{
    //    List<ValidationIssue> Issues
    //    {
    //        get;
    //    }
    //    IValidation AddIssue(ValidationIssue issue);
    //    bool Valid { get; }
    //}

    public static class IValidationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validation"></param>
        /// <param name="separator">If null, NewLine is used.</param>
        /// <returns>Null if no reasons</returns>
        public static string FailReasons(this ValidationContext validation, string separator = null)
        {
            if (separator == null) separator = Environment.NewLine;
            if (validation.Issues == null || !validation.Issues.Any()) return null;
            return validation.Issues.Select(i => i.Message).Aggregate((x, y) => x + Environment.NewLine + y);
        }
    }
    //public struct LazyValidation : IValidation
    //{

    //    Func<ValidationContext> factory;

    //    public LazyValidation(Func<ValidationContext> factory) { this.factory = factory; }

    //    public IValidation AddIssue(ValidationIssue issue)
    //    {
    //        var vc = new ValidationContext();
    //        return vc.AddIssue(issue);
    //    }
    //}

    public static class Validate
    {
        public static ValidationContext For(object obj)
        {
            return new ValidationContext(obj);
        }
    }

    public struct ValidationContext // : IValidation // RENAME to ValidationOperation?
    {
        #region Construction

        public ValidationContext(object obj = null, object validationKind = null) { this.Object = obj; ValidationKind = validationKind; issues = null; }

        #endregion

        #region Properties

        public object Object { get; set; }

        // REVIEW: Rethink this here.  Move it to IValidateEx if it is really necessary.  Or change to Context
        // RENAME to Context
        public object ValidationKind { get; set; }

        private List<ValidationIssue> issues { get; set; }
        public IEnumerable<ValidationIssue> Issues => issues;

        #endregion

        #region Derived

        public bool Valid => issues == null || issues.Count == 0;
        public bool HasIssues => !Valid;

        #endregion

        #region Methods

        public ValidationContext AddIssue(ValidationIssue issue)
        {
            if (issues == null) { issues = new List<ValidationIssue>(); }
            issues.Add(issue);
            return this;
        }
      
        #endregion
    }

    //public static class ValidationContextExtensions
    //{
    //    public static ValidationContext StartValidate(this ref ValidationContext vc)
    //    {
    //        if (vc == null) vc = new ValidationContext();
    //        return vc;
    //    }
    //}

}
