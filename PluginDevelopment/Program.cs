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
//Share a record Read,Write and Append privileges with Team
private void SharePrivileges(string targetEntityName, Guid targetRecordID, Guid teamID, bool read_Access, bool write_Access, bool append_Access, IOrganizationService orgService)
{
try
{
//Get User or Team reference and Target Entity and record ID that needs to be shared.      
var recordRef = new EntityReference(targetEntityName, targetRecordID);
var teamRef = new EntityReference("team", teamID);
AccessRights Access_Rights = new AccessRights();
Access_Rights = AccessRights.None;
//Read Access           
if (read_Access == true)
Access_Rights = AccessRights.ReadAccess;
//Write Access (or) Read, Write Access        
if (write_Access == true)
if (Access_Rights == AccessRights.None)
Access_Rights = AccessRights.WriteAccess;
else
Access_Rights = Access_Rights | AccessRights.WriteAccess;
//Append Access or all or any two accesses         
if (append_Access == true)
if (Access_Rights == AccessRights.None)
Access_Rights = AccessRights.AppendToAccess | AccessRights.AppendAccess;
else
Access_Rights = Access_Rights | AccessRights.AppendToAccess | AccessRights.AppendAccess;
var grantAccess = new GrantAccessRequest
{
PrincipalAccess = new PrincipalAccess
{
AccessMask = Access_Rights,
Principal = teamRef
},
Target = recordRef
};
// Execute the Request      
orgService.Execute(grantAccess);
}
catch (Exception ex)
{
throw new Exception("An error occured while applying Sharing rules for the record." + ex.Message);
}
}
//Code to modify privileges for the target record and team   
private void ModifyAccess(string targetEntityName, Guid targetRecordID, Guid teamID, IOrganizationService orgService)
{
try
{
//Get User or Team reference and Target Entity and record ID that needs to be shared.          
var RecordReference = new EntityReference(targetEntityName, targetRecordID);
var teamRef = new EntityReference("team", teamID);
AccessRights accessRights = new AccessRights();
accessRights = AccessRights.DeleteAccess;
var modifyAcess = new ModifyAccessRequest
{
PrincipalAccess = new PrincipalAccess
{
AccessMask = accessRights,
Principal = teamRef
},
Target = RecordReference
};
// Execute the Request   
orgService.Execute(modifyAcess);
}
catch (Exception ex)
{
throw new Exception("An error occured in Modifying access." + ex.Message);
}
}
//Code to remvove the sharing privileges for the target record and team    
private void RevokeAccess(string targetEntityName, Guid targetRecordID, Guid teamID, IOrganizationService orgService)
{
try
{
//Get User or Team reference and Target Entity and record ID that needs to be shared.        
var recordRef = new EntityReference(targetEntityName, targetRecordID);
var teamRef = new EntityReference("team", teamID);
var revokeAcess = new RevokeAccessRequest
{
Revokee = teamRef,
Target = recordRef
};
// Execute the Request        
orgService.Execute(revokeAcess);
}
catch (Exception ex)
{
throw new Exception("An error occured in Revoking access." + ex.Message);
}
}

//Contains the data that is needed to retrieve the access rights of the specified security principal (team or user) to the specified record.

public RetrievePrincipalAccessResponse RetrieveRights(Guid userId, Guid objectId, string entityName)
{
try
{
// Requête d'accès
var request = new RetrievePrincipalAccessRequest();
request.Principal = new EntityReference("systemuser", userId);
request.Target = new EntityReference(entityName, objectId);

return (RetrievePrincipalAccessResponse)service.Execute(request);

//Contains the data that is needed to retrieve all security principals (users or teams) that have access to, and access rights for, the specified record.

var accessRequest = new RetrieveSharedPrincipalsAndAccessRequest
{
    Target = leadReference
};

// The RetrieveSharedPrincipalsAndAccessResponse returns an entity reference
// that has a LogicalName of "user" when returning access information for a
// "team."

var accessResponse = (RetrieveSharedPrincipalsAndAccessResponse)
    svc.Execute(accessRequest);
Console.WriteLine("The following have the specified granted access to the lead.");

foreach (var principalAccess in accessResponse.PrincipalAccesses)
{
    Console.WriteLine("\t{0}:\r\n\t\t{1}",
        GetEntityReferenceString(principalAccess.Principal),
        principalAccess.AccessMask);
}
Console.WriteLine();

}
catch (Exception error)
{
throw new Exception("Error while checking rigths: " + error.Message);
}
}
//Contains the data that’s needed to set the state of an entity record.


public static SetStateRequest1()
{
// Create the Request Object
var state = new SetStateRequest();
// Set the Request Object's Properties
state.State = new OptionSetValue((int)IncidentState.Active);
state.Status = 
    new OptionSetValue((int)incident_statuscode.WaitingforDetails);

// Point the Request to the case whose state is being changed
state.EntityMoniker = caseReference;

// Execute the Request
var stateSet = (SetStateResponse)svc.Execute(state);

// Check if the state was successfully set
var incident = svc.Retrieve(Incident.EntityLogicalName, 
    _caseIncidentId, new ColumnSet(allColumns: true)).ToEntity&lt;Incident&gt;();

if (incident.StatusCode.Value == (int)incident_statuscode.WaitingforDetails)
{
    Console.WriteLine("Record state set successfully.");
}
else
{
    Console.WriteLine("The request to set the record state failed.");
}
}


static void Main(string[] args)
{

}
}
}
