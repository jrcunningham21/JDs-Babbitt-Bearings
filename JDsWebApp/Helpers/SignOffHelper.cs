using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JDsWebApp
{
    public static class CustomHtmlHelpers
    {
        public static MvcHtmlString SignOffHelper<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string id)
        {
            var label = helper.LabelFor(expression);
            var attribs = new Dictionary<string, object>();

            attribs.Add("readonly", "true");
            attribs.Add("class", "form-control signoffText");
            attribs.Add("id", id);
            var control = helper.TextBoxFor(expression, attribs);

            var glphicon = control.ToString().Contains("value=\"\"") ? "glyphicon-minus" : "glyphicon-ok";

            var btn = "<a id=\"" + id + "SO\" class=\"btn btn-primary\" onclick=\"OnClickSignOff('" + id + "')\"><span class=\"glyphicon " + glphicon + "\" aria-hidden=\"true\"></span> SignOff</a>";

            return new MvcHtmlString(label + "<div class=\"input-group\">" + control + "<span class=\"input-group-btn\">" + btn + "</span></div>");
        }

        /// <summary>
        /// Static method to convert an enum into a Dictionary.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static Dictionary<string, object> ToDictionary(Type @type)
        {
            return Enum.GetValues(@type).Cast<object>().ToDictionary(e => Enum.GetName(@type, e));

        }

        /// <summary>
        /// HTML Bootstrap radio button helper.
        /// </summary>
        /// <param name="expression">The lambda expression to evaluate.</param>
        /// <param name="id">Used to name the button group and the radio buttons.</param>
        /// <param name="kvPairs">A dictionary of &lt;string,object&gt; pairs, one for each button.</param>
        /// <returns>A button group with radio buttons, each valued by its corresponding value and surrounded by a label with its corresponding key.</returns>
        /// <remarks>
        /// To enable the button group, remove the disabled class from the labels (the disabled property of the radio buttons themselves is not used), e.g., $("input[type='radio']").parent().removeClass("disabled").
        /// To disable the button group but still show the selection, add the disabled class skipping labels with the active class, e.g., $("input[type='radio']").parent(":not(.active)").addClass("disabled").
        /// The key-value pairs dictionary is optional and defaults to a "No|Yes" toggle button for expressions with return type Boolean and to Enum names for expressions with return type Enum.
        /// </remarks>
        public static MvcHtmlString ToggleButtonFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string id, Dictionary<string, object> kvPairs = null)
        {
            if (kvPairs == null)
            {
                if (expression.ReturnType == typeof(bool))
                {
                    kvPairs = new Dictionary<string, object>();

                    kvPairs.Add("No", false);
                    kvPairs.Add("Yes", true);
                }
                else if (expression.ReturnType.BaseType == typeof(Enum))
                {
                    kvPairs = ToDictionary(expression.ReturnType);
                }
                else
                    throw new ArgumentNullException("kvPairs", "A null dictionary is only allowed for expresions with Boolean or Enum return types.");
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div class=\"btn-group\" data-toggle=\"buttons\" id=\"{0}Group\">\n", id);

            Type type = expression.ReturnType;

            foreach (var kv in kvPairs)
            {
                var button = helper.RadioButtonFor(expression, kv.Value, new { id = id + kv.Key }).ToString();

                sb.AppendFormat("<label class=\"btn btn-primary{0}\">\n{1}{2}\n</label>\n", button.Contains("checked=\"checked\"") ? " active" : " disabled", button, kv.Key);
            }

            sb.AppendFormat("</div>\n", id);

            return new MvcHtmlString(sb.ToString());
        }
    }
}