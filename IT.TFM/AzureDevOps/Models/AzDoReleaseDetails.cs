namespace AzureDevOps.Models
{
    public class AzDoReleaseDetails
    {
        public string? source { get; set; }
        public int revision { get; set; }
        public object? description { get; set; }
        public Createdby? createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public Modifiedby? modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
        public bool isDeleted { get; set; }
        public bool isDisabled { get; set; }
        public LastRelease? lastRelease { get; set; }
        public Variables? variables { get; set; }
        public int[] variableGroups { get; set; } = [];
        public Environment[] environments { get; set; } = [];
        public Artifact[] artifacts { get; set; } = [];
        public object[] triggers { get; set; } = [];
        public string? releaseNameFormat { get; set; }
        public object[] tags { get; set; } = [];
        public Properties? properties { get; set; }
        public int id { get; set; }
        public string? name { get; set; }
        public string? path { get; set; }
        public object? projectReference { get; set; }
        public string? url { get; set; }
        public Links? _links { get; set; }
    }

    public class _Links
    {
        public Avatar? avatar { get; set; }
    }

    public class _Links1
    {
        public Avatar? avatar { get; set; }
    }

    public class LastRelease
    {
        public int id { get; set; }
        public string? name { get; set; }
        public object[] artifacts { get; set; } = [];
        public string? description { get; set; }
        public Releasedefinition? releaseDefinition { get; set; }
        public DateTime? createdOn { get; set; }
        public Createdby? createdBy { get; set; }
    }

    public class Releasedefinition
    {
        public int id { get; set; }
        public object? projectReference { get; set; }
    }

    public class Approotfolder
    {
        public string? value { get; set; }
    }

    public class Productfamilyname
    {
        public string? value { get; set; }
    }

    public class Temprootfolder
    {
        public string? value { get; set; }
    }

    public class Webservicename
    {
        public string? value { get; set; }
    }

    public class Websitenamelayer7
    {
        public string? value { get; set; }
    }

    public class Properties
    {
        public Definitioncreationsource? DefinitionCreationSource { get; set; }
        public Integratejiraworkitems? IntegrateJiraWorkItems { get; set; }
        public Integrateboardsworkitems? IntegrateBoardsWorkItems { get; set; }
    }

    public class Definitioncreationsource
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }

    public class Integratejiraworkitems
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }

    public class Integrateboardsworkitems
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }
    public class Environment
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int rank { get; set; }
        public Owner? owner { get; set; }
        public Variables? variables { get; set; }
        public object[] variableGroups { get; set; } = [];
        public Predeployapprovals? preDeployApprovals { get; set; }
        public Deploystep? deployStep { get; set; }
        public Postdeployapprovals? postDeployApprovals { get; set; }
        public Deployphas[] deployPhases { get; set; } = [];
        public Environmentoptions? environmentOptions { get; set; }
        public object[] demands { get; set; } = [];
        public Condition[] conditions { get; set; } = [];
        public Executionpolicy? executionPolicy { get; set; }
        public object[] schedules { get; set; } = [];
        public Currentrelease? currentRelease { get; set; }
        public Retentionpolicy? retentionPolicy { get; set; }
        public Properties1? properties { get; set; }
        public Predeploymentgates? preDeploymentGates { get; set; }
        public Postdeploymentgates? postDeploymentGates { get; set; }
        public object[] environmentTriggers { get; set; } = [];
        public string? badgeUrl { get; set; }
    }

    public class Owner
    {
        public string? displayName { get; set; }
        public string? url { get; set; }
        public Links1? _links { get; set; }
        public string? id { get; set; }
        public string? uniqueName { get; set; }
        public string? imageUrl { get; set; }
        public string? descriptor { get; set; }
    }

    public class Apppoolidentityusername
    {
        public string? value { get; set; }
    }

    public class Predeployapprovals
    {
        public Approval[] approvals { get; set; } = [];
        public Approvaloptions? approvalOptions { get; set; }
    }

    public class Approvaloptions
    {
        public object? requiredApproverCount { get; set; }
        public bool releaseCreatorCanBeApprover { get; set; }
        public bool autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }
        public bool enforceIdentityRevalidation { get; set; }
        public int timeoutInMinutes { get; set; }
        public string? executionOrder { get; set; }
    }

    public class Approval
    {
        public int rank { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int id { get; set; }
        public Approver? approver { get; set; }
    }

    public class Approver
    {
        public string? displayName { get; set; }
        public string? url { get; set; }
        public Links1? _links { get; set; }
        public string? id { get; set; }
        public string? uniqueName { get; set; }
        public string? imageUrl { get; set; }
        public bool isContainer { get; set; }
        public string? descriptor { get; set; }
    }

    public class Deploystep
    {
        public int id { get; set; }
    }

    public class Postdeployapprovals
    {
        public Approval[] approvals { get; set; } = [];
        public Approvaloptions? approvalOptions { get; set; }
    }

    public class Environmentoptions
    {
        public string? emailNotificationType { get; set; }
        public string? emailRecipients { get; set; }
        public bool skipArtifactsDownload { get; set; }
        public int timeoutInMinutes { get; set; }
        public bool enableAccessToken { get; set; }
        public bool publishDeploymentStatus { get; set; }
        public bool badgeEnabled { get; set; }
        public bool autoLinkWorkItems { get; set; }
        public bool pullRequestDeploymentEnabled { get; set; }
    }

    public class Executionpolicy
    {
        public int concurrencyCount { get; set; }
        public int queueDepthCount { get; set; }
    }

    public class Currentrelease
    {
        public int id { get; set; }
        public string? url { get; set; }
    }

    public class Retentionpolicy
    {
        public int daysToKeep { get; set; }
        public int releasesToKeep { get; set; }
        public bool retainBuild { get; set; }
    }

    public class Properties1
    {
        public Linkboardsworkitems? LinkBoardsWorkItems { get; set; }
        public Boardsenvironmenttype? BoardsEnvironmentType { get; set; }
    }

    public class Linkboardsworkitems
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }

    public class Boardsenvironmenttype
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }

    public class Predeploymentgates
    {
        public int id { get; set; }
        public object? gatesOptions { get; set; }
        public object[] gates { get; set; } = [];
    }

    public class Postdeploymentgates
    {
        public int id { get; set; }
        public object? gatesOptions { get; set; }
        public object[] gates { get; set; } = [];
    }

    public class Deployphas
    {
        public Deploymentinput? deploymentInput { get; set; }
        public int rank { get; set; }
        public string? phaseType { get; set; }
        public string? name { get; set; }
        public object? refName { get; set; }
        public Workflowtask[] workflowTasks { get; set; } = [];
    }

    public class Deploymentinput
    {
        public Parallelexecution? parallelExecution { get; set; }
        public Agentspecification? agentSpecification { get; set; }
        public bool skipArtifactsDownload { get; set; }
        public Artifactsdownloadinput? artifactsDownloadInput { get; set; }
        public int queueId { get; set; }
        public object[] demands { get; set; } = [];
        public bool enableAccessToken { get; set; }
        public int timeoutInMinutes { get; set; }
        public int jobCancelTimeoutInMinutes { get; set; }
        public string? condition { get; set; }
    }

    public class Parallelexecution
    {
        public string? parallelExecutionType { get; set; }
    }

    public class Agentspecification
    {
        public string? identifier { get; set; }
    }

    public class Artifactsdownloadinput
    {
        public object[] downloadInputs { get; set; } = [];
    }

    public class Workflowtask
    {
        public Environment? environment { get; set; }
        public string? taskId { get; set; }
        public string? version { get; set; }
        public string? name { get; set; }
        public string? refName { get; set; }
        public bool enabled { get; set; }
        public bool alwaysRun { get; set; }
        public bool continueOnError { get; set; }
        public int timeoutInMinutes { get; set; }
        public int retryCountOnTaskFailure { get; set; }
        public string? definitionType { get; set; }
        public string? condition { get; set; }
        public Inputs? inputs { get; set; }
    }

    public class Inputs
    {
        public string? ConnectedServiceName { get; set; }
        public string? KeyVaultName { get; set; }
        public string? SecretsFilter { get; set; }
        public string? RunAsPreJob { get; set; }
        public string? ApiServer { get; set; }
        public string? AppRootFolder { get; set; }
        public string? Password { get; set; }
        public string? ProductFamilyName { get; set; }
        public string? TempRootFolder { get; set; }
        public string? Username { get; set; }
        public string? WebServiceName { get; set; }
        public string? WebsiteNameLayer7 { get; set; }
        public string? _lock { get; set; }
        public string? SRAutomationPAT { get; set; }
    }

    public class Condition
    {
        public string? name { get; set; }
        public string? conditionType { get; set; }
        public string? value { get; set; }
        public object? result { get; set; }
    }

    public class Artifact
    {
        public string? sourceId { get; set; }
        public string? type { get; set; }
        public string? alias { get; set; }
        public Definitionreference? definitionReference { get; set; }
        public bool isPrimary { get; set; }
        public bool isRetained { get; set; }
    }

    public class Definitionreference
    {
        public Artifactsourcedefinitionurl? artifactSourceDefinitionUrl { get; set; }
        public Defaultversionbranch? defaultVersionBranch { get; set; }
        public Defaultversionspecific? defaultVersionSpecific { get; set; }
        public Defaultversiontags? defaultVersionTags { get; set; }
        public Defaultversiontype? defaultVersionType { get; set; }
        public Definition? definition { get; set; }
        public Definitions? definitions { get; set; }
        public Ismultidefinitiontype? IsMultiDefinitionType { get; set; }
        public Project? project { get; set; }
        public Repository? repository { get; set; }
        public Branches? branches { get; set; }
        public Checkoutnestedsubmodules? checkoutNestedSubmodules { get; set; }
        public Checkoutsubmodules? checkoutSubmodules { get; set; }
        public Fetchdepth? fetchDepth { get; set; }
        public Gitlfssupport? gitLfsSupport { get; set; }
    }

    public class Artifactsourcedefinitionurl
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Defaultversionbranch
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Defaultversionspecific
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Defaultversiontags
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Defaultversiontype
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Definitions
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Ismultidefinitiontype
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Project
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Repository
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Branches
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Checkoutnestedsubmodules
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Checkoutsubmodules
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Fetchdepth
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class Gitlfssupport
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }
}
