using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Obfuscator.Attribute
{
    /// <summary>
    /// Add this to class members to obfuscate them anyway with a new name '_ObfuscateTo', although the settings did not allow to.
    /// For example if you do not want to obfuscate all public methods beside some specific.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event)]
    public class ObfuscateAnywayAttribute : System.Attribute
    {
#pragma warning disable
        private string obfuscateTo;

        public ObfuscateAnywayAttribute(String _ObfuscateTo)
        {
            this.obfuscateTo = _ObfuscateTo;
        }
#pragma warning restore
    }
}