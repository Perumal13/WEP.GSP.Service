using System;
using System.Reflection;

namespace WEP.GSP.AZFNTrigger.RestAPI.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}