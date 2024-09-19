using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

public class FollowupPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        // Obtain the tracing service
        ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        // Obtain the execution context from the service provider
        IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        // The InputParameters collection contains all the data passed in the message request
        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity)
        {
            // Obtain the IOrganizationService instance which you will need for web service calls
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Ensure that the necessary attributes exist before attempting to access them
                if (entity.Contains("lastname") && entity.Contains("firstname"))
                {
                    string lastName = entity.GetAttributeValue<string>("lastname");
                    string firstName = entity.GetAttributeValue<string>("firstname");

                    // Update the fullname attribute based on firstname and lastname
                    entity["jobtitle"] = "Analyst";

                    // Update the entity in CRM
                    service.Update(entity);
                }
                else
                {
                    tracingService.Trace("FollowupPlugin: Firstname or Lastname is missing in the input entity.");
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                tracingService.Trace("FollowupPlugin: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
            }
            catch (Exception ex)
            {
                tracingService.Trace("FollowupPlugin: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An unexpected error occurred in FollowUpPlugin.", ex);
            }
        }
        else
        {
            tracingService.Trace("FollowupPlugin: Target entity is not provided or is not of type Entity.");
        }
    }
}
