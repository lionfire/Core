//using System;
//using System.Collections.Generic;
//using System.Text;
//using LionFire.MultiTyping;

//namespace LionFire.Framing
//{
//    public interface IValidationError
//    {
//        public string Message { get; }

//    }
//    public class ValidationError : IValidationError
//    {

//    }
//    public interface IFrame
//    {
//        void Validate()
//    }

//    public class Frame
//    {
//        [SetOnce]
//        public Frame Parent { get; set; } 

        

//        public (bool validated, IEnumerable<IValidationError> errors) Validate()
//        {
//            return (true, null);
//        }

//        public void OnChanged(object changed)
//        {
//            FrameChanged?.Invoke(changed);
//        }
//        public event Action<Frame, object> FrameChanged;
//    }

//    public enum FrameStatus
//    {
//        Unspecified,
//        Satisfied,
//        SatisfiedIfNoChanges,
//    }
//    public class FrameChild
//    {

//    }

//    public class ServiceConfiguration : MultiType, Frame
//    {
//        public IServiceCollection ServiceCollection { get; set; }

//        public List<object> Children { get; set; } = new List<object>();

//        public 
//    }

//    public class Initializable : MultiType, Frame
//    {
//        public IServiceCollection ServiceCollection { get; set; }

//    }
//}
