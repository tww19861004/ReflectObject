using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReflectObject
{
    public class Obj
    {
        public string fieldname { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }
    public class ComplexObject
    {
        public int Id { get; set; }
        public int? Id1 { get; set; }
        public string Name { get; set; }
        public Child c1 { get; set; }
        public List<Child> c1lst { get; set; }
    }
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ComplexObject co = new ReflectObject.ComplexObject() { Id = 1,Name = "tww"};
            co.c1 = new ReflectObject.Child() { Id = 2, Name = "tyc" };
            co.c1lst = new List<Child>();
            co.c1lst.Add(new ReflectObject.Child() { Id= 3,Name="thf"});

            List<Obj> res = new List<ReflectObject.Obj>();
            TraversalAllTypes(co, res);

            StringBuilder svb = new StringBuilder();
            svb.Append("<table border=\"1\" style=\"table-layout:fixed;word-break:break-all;width:90%;\">");
            svb.Append("<tr>");
            svb.Append("<td style=\"width:30%;word-wrap:break-word;\">");
            svb.Append("字段名称");
            svb.Append("</td>");
            svb.Append("<td style=\"width:30%;word-wrap:break-word;\">");
            svb.Append("字段类型");
            svb.Append("</td>");
            svb.Append("<td style=\"width:30%;word-wrap:break-word;\">");
            svb.Append("值");
            svb.Append("</td>");
            svb.Append("</tr>");

            //直分销都有的字段
            foreach (var item in res)
            {
                svb.Append("<tr>");
                svb.Append("<td>");
                svb.Append($"{item.fieldname}");
                svb.Append("</td>");
                svb.Append("<td>");
                svb.Append($"{item.type}");
                svb.Append("</td>");
                svb.Append("<td>");
                svb.Append($"{item.value}");
                svb.Append("</td>");
                svb.Append("</tr>");
            }

            svb.Append("</table>");
            Label1.Text = svb.ToString();
        }
        private void TraversalAllTypes(object t, List<Obj> res)
        {
            //结束递归
            if (null == t)
            {
                return;
            }
            if (res == null)
            {
                res = new List<Obj>();
            }
            var properties = t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo p in properties)//获得类型的属性字段  
            {
                //值类型 或者 string 直接比较
                if (p.PropertyType.IsValueType
                    || p.PropertyType.FullName == "System.String")
                {
                    res.Add(new Obj()
                    {
                        fieldname = p.Name,
                        type = p.PropertyType.FullName,
                        value = (p.GetValue(t) ?? string.Empty).ToString()
                    });
                }
                else if (p.PropertyType.IsGenericType)
                {
                    if (p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        res.Add(new Obj()
                        {
                            fieldname = p.Name,
                            type = p.PropertyType.FullName,
                            value = (p.GetValue(t) ?? string.Empty).ToString()
                        });
                    }
                    else
                    {
                        res.Add(new Obj()
                        {
                            fieldname = p.Name,
                            type = p.PropertyType.FullName,
                            value = Newtonsoft.Json.JsonConvert.SerializeObject(p.GetValue(t), Newtonsoft.Json.Formatting.Indented)
                        });
                    }
                }
                else
                {
                    TraversalAllTypes(p.GetValue(t), res);
                }
            }
        }
    }
}