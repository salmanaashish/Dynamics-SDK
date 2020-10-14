using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
namespace PluginDevelopment
{
    class Program : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data
           

         if (context.InputParameters.Contains("Target"))
            {

                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                try
                {

                    // Create a task activity to follow up with the account customer in 7 days
                    Entity followup = new Entity("task");
                    followup["subject"] = "Send e-mail to the new customer.";
                    followup["description"] ="Follow up with the customer. Check if there are any new issues that need resolution.";
    

               followup["scheduledstart"] = DateTime.Now;
                    followup["scheduledend"] = DateTime.Now.AddDays(2);
                    followup["category"] = context.PrimaryEntityName;

                    // Refer to the contact in the task activity.
                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "contact";
                        followup["regardingobjectid"] =
                           new EntityReference(regardingobjectidType, regardingobjectid);
                    }
                    // Define variables to store Preimage and Postimage  string pretopic = string.Empty; 
                    string posttopic = string.Empty;
                    string pretopic = string.Empty;

                    // get PreImage from Context

                    if (context.PreEntityImages.Contains("LeadTopicImage") && context.PreEntityImages["LeadTopicImage"] is Entity)
                    {

                        Entity preMessageImage = (Entity)context.PreEntityImages["LeadTopicImage"];
                        // get topic field value before database update perform
                        pretopic = (String)preMessageImage.Attributes["subject"];

                    }


                    // get PostImage from Context
                    if (context.PostEntityImages.Contains("LeadTopicImage") &&
                           context.PostEntityImages["LeadTopicImage"] is Entity)

                    {

                        Entity postMessageImage = (Entity)context.PostEntityImages["LeadTopicImage"];

                        // get topic field value after database update performed
                        posttopic = (String)postMessageImage.Attributes["subject"];

                    }

                    // Obtain the organization service reference.
                    IOrganizationServiceFactory serviceFactory =
                       (IOrganizationServiceFactory)serviceProvider.GetService
                       (typeof(IOrganizationServiceFactory));
                    IOrganizationService service =
                       serviceFactory.CreateOrganizationService(context.UserId);

                    // Create the followup activity
                    service.Create(followup);
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
        static void Main(string[] args)
        {

        }
    }
}
