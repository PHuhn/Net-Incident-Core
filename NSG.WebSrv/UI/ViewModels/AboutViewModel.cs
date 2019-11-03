using System;
using System.Collections.Generic;
using System.Reflection;
//
namespace NSG.WebSrv.UI.ViewModels
{
    public class AboutViewModel
    {
        public string Version { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        //
        /// <summary>
        /// Constructor, get the values from the assembly.
        /// </summary>
        public AboutViewModel()
        {
            Assembly _asm = Assembly.GetExecutingAssembly();
            Version = ((System.Reflection.AssemblyFileVersionAttribute)_asm.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false)[0]).Version;
            Product = ((System.Reflection.AssemblyProductAttribute)_asm.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false)[0]).Product;
            Copyright = ((System.Reflection.AssemblyCopyrightAttribute)_asm.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false)[0]).Copyright;
            Company = ((System.Reflection.AssemblyCompanyAttribute)_asm.GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), false)[0]).Company;
            Description = ((System.Reflection.AssemblyDescriptionAttribute)_asm.GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false)[0]).Description;
        }
        //
        /// <summary>
        /// Override the 'to string' method.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "Product: {0}, Version: {1}, Company: {2}, Copyright: {3}",
                Product, Version, Company, Copyright );
        }
        //
    }
}
//