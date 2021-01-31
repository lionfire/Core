using System;

namespace LionFire.Instantiating
{
    /// <summary>
    /// Holds references to Template and Parameters used to create this instance
    /// </summary>
    /// <typeparam name="TTemplate"></typeparam>
    /// <typeparam name="TParameters"></typeparam>
    public class InstantiationAwareBase<TTemplate, TParameters> : ITemplateInstance<TTemplate, TParameters>
       where TTemplate : class, ITemplate
       where TParameters : class, ITemplateParameters
    {
        #region Parameters

        ITemplateParameters IParameterizedTemplateInstance.Parameters { get => parameters; set => parameters = (TParameters)value; }

        [SetOnce]
        public TParameters Parameters
        {
            get => parameters;
            set
            {
                if (parameters != null) throw new AlreadyException("Can only be set once");
                parameters = value;
            }
        }
        private TParameters parameters;

        #endregion

        #region Template

        public TTemplate Template
        {
            get => template;
            set
            {
                if (ReferenceEquals(value, template)) return;
                if (template != null) throw new NotSupportedException("Can only be set once");
                template = value;
            }
        }
        private TTemplate template;

        #endregion
    }

}
