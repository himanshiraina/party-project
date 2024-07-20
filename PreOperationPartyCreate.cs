using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk.Extensions;

namespace PartyPackageProject
{
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class PreOperationPartyCreate : PluginBase
    {
        public PreOperationPartyCreate(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(PreOperationPartyCreate))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }



        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            localPluginContext.Trace("Hello1");
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;

            // TODO: Implement your custom business logic


            var partyEntity = context.InputParameters["Target"] as Entity;

            EntityReference ef = (EntityReference)partyEntity["contoso_contact"];
            Guid id = ef.Id;

            string fetchString = $"<fetch output-format='xml-platform' distinct='false' " +
                "version='1.0' mapping='logical' " +
                "aggregate='true'>" +
                "<entity name='contoso_installationrequests'>" +
                "<attribute name='contoso_contact' alias='Count' aggregate='count' />" +
                "<filter type='and'>" +
                $"<condition attribute='contoso_contact' operator='eq' value='{id}'/>" +
                "<filter type='or' >" +
                "<condition attribute='contoso_installationstatus' operator='eq' value='330650001'/>" +
                "<condition attribute='contoso_installationstatus' operator='eq' value='330650000'/>" +
                "</filter>" +
                "</filter>" +
                "</entity>" +
                "</fetch>";


            var response = localPluginContext.InitiatingUserService.RetrieveMultiple(new FetchExpression(fetchString));
            int IncompleteRequestCount = (int)((AliasedValue)response.Entities[0]["Count"]).Value;

            localPluginContext.Trace("Locked Permit count : " + IncompleteRequestCount);
            if (IncompleteRequestCount > 0)
            {
                throw new InvalidPluginExecutionException("Bandha ko vaise bhi bahut kaam hai, chod do usko");
            }
        }
    }
}
