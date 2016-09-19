using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Salesforce.VisualStudio.Services.ConnectedService.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Shell = Microsoft.VisualStudio.Shell;

namespace Salesforce.VisualStudio.Services.ConnectedService.Utilities
{
    /// <summary>
    /// A utility class that invokes the code generators to produce the scaffolded code that is added to the user's project.
    /// </summary>
    internal static class GeneratedCodeHelper
    {
        private static ITextTemplating TextTemplating
        {
            get { return (ITextTemplating)Shell.Package.GetGlobalService(typeof(STextTemplating)); }
        }

        public static async Task AddGeneratedCodeAsync(
            ConnectedServiceHandlerContext context,
            Project project,
            string templateFileName,
            string outputDirectory,
            Func<ITextTemplatingSessionHost, IEnumerable<ITextTemplatingSession>> getSessions,
            Func<IPreprocessedT4Template> getPreprocessedT4Template,
            Func<ITextTemplatingSession, string> getArtifactName)
        {
            string templatePath = Path.Combine(
                    RegistryHelper.GetCurrentUsersVisualStudioLocation(),
                    "Templates\\ConnectedServiceTemplates\\Visual C#\\Salesforce",
                    templateFileName + ".tt");
            bool useCustomTemplate = File.Exists(templatePath);

            SalesforceConnectedServiceInstance salesforceInstance = (SalesforceConnectedServiceInstance)context.ServiceInstance;
            salesforceInstance.TelemetryHelper.TrackCodeGeneratedEvent(salesforceInstance, templateFileName, useCustomTemplate);

            ITextTemplating textTemplating = GeneratedCodeHelper.TextTemplating;
            ITextTemplatingSessionHost sessionHost = (ITextTemplatingSessionHost)textTemplating;
            Func<ITextTemplatingSession, string> generateText;

            if (useCustomTemplate)
            {
                // The current user has a customized template, process and use it.
                string customTemplate = File.ReadAllText(templatePath);
                generateText = (session) =>
                {
                    sessionHost.Session = session;
                    return textTemplating.ProcessTemplate(templatePath, customTemplate);
                };
            }
            else
            {
                // No customized template exists for the current user, use the preprocessed one for increased performance.
                generateText = (session) =>
                {
                    IPreprocessedT4Template t4Template = getPreprocessedT4Template();
                    t4Template.Session = session;
                    t4Template.Initialize();
                    return t4Template.TransformText();
                };
            }

            foreach (ITextTemplatingSession session in getSessions(sessionHost))
            {
                string generatedText = generateText(session);
                string tempFileName = GeneratedCodeHelper.CreateTempFile(generatedText);
                string targetPath = Path.Combine(outputDirectory, getArtifactName(session) + ".Generated.cs");
                await context.HandlerHelper.AddFileAsync(tempFileName, targetPath);
            }
        }

        private static string CreateTempFile(string contents)
        {
            string tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, contents);

            return tempFileName;
        }
    }
}
