using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace WebApplication5.Common
{
    public class RemoteClientServer
    {        
        /// <summary>
        /// RemoteClientServerAttribute Class
        /// </summary>
        public class RemoteClientServerAttribute : RemoteAttribute
        {
            #region isValid Method
            /// <summary>Returns true if ... is valid.</summary>
            /// <param name="value">The value to validate.</param>
            /// <param name="validationContext">The context information about the validation operation.</param>
            /// <returns>An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class.</returns>
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                Type controller = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(type => type.Name.ToLower() == string.Format("{0}Controller",
                        this.RouteData["controller"].ToString()).ToLower());
                if (controller != null)
                {
                    MethodInfo action = controller.GetMethods()
                        .FirstOrDefault(method => method.Name.ToLower() ==
                            this.RouteData["action"].ToString().ToLower());
                    if (action != null)
                    {
                        object instance = Activator.CreateInstance(controller);
                        object response = action.Invoke(instance, new object[] { value });
                        if (response is JsonResult)
                        {
                            object jsonData = ((JsonResult)response).Data;
                            if (jsonData is bool)
                            {
                                return (bool)jsonData ? ValidationResult.Success :
                                    new ValidationResult(this.ErrorMessage);
                            }
                        }
                    }
                }
                return ValidationResult.Success;
            }
            #endregion

            #region "RemoteClientServerAttribute Method"
            /// <summary>
            /// RemoteClientServerAttribute Method
            /// </summary>
            /// <param name="routeName"></param>
            public RemoteClientServerAttribute(string routeName)
                : base(routeName)
            {
            }
            #endregion

            #region "RemoteClientServerAttribute Method"
            /// <summary>
            /// RemoteClientServerAttribute Method
            /// </summary>
            /// <param name="action"></param>
            /// <param name="controller"></param>
            public RemoteClientServerAttribute(string action, string controller)
                : base(action, controller)
            {
            }
            #endregion

            #region "RemoteClientServerAttribute Method"
            /// <summary>
            /// RemoteClientServerAttribute Method
            /// </summary> 
            /// <param name="action"></param>
            /// <param name="controller"></param>
            /// <param name="areaName"></param>
            public RemoteClientServerAttribute(string action, string controller,
                string areaName) : base(action, controller, areaName)
            {
            }
            #endregion
        }
    }
}