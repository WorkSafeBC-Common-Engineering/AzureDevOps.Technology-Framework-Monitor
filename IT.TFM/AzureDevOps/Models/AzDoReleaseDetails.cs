namespace AzureDevOps.Models
{
    public class AzDoReleaseDetails
    {
        public string? Source { get; set; }
        public int Revision { get; set; }
        public object? Description { get; set; }
        public Createdby? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Modifiedby? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
        public LastRelease? LastRelease { get; set; }
        public Variables? Variables { get; set; }
        public int[] VariableGroups { get; set; } = [];
        public Environment[] Environments { get; set; } = [];
        public Artifact[] Artifacts { get; set; } = [];
        public object[] Triggers { get; set; } = [];
        public string? ReleaseNameFormat { get; set; }
        public object[] Tags { get; set; } = [];
        public Properties? Properties { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public object? ProjectReference { get; set; }
        public string? Url { get; set; }
        public Links? Links { get; set; }
    }

    public class LastRelease
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public object[] Artifacts { get; set; } = [];
        public string? Description { get; set; }
        public Releasedefinition? ReleaseDefinition { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Createdby? CreatedBy { get; set; }
    }

    public class Releasedefinition
    {
        public int Id { get; set; }
        public object? ProjectReference { get; set; }
    }

    public class Approotfolder
    {
        public string? Value { get; set; }
    }

    public class Productfamilyname
    {
        public string? Value { get; set; }
    }

    public class Temprootfolder
    {
        public string? Value { get; set; }
    }

    public class Webservicename
    {
        public string? Value { get; set; }
    }

    public class Websitenamelayer7
    {
        public string? Value { get; set; }
    }

    public class Properties
    {
        public Definitioncreationsource? DefinitionCreationSource { get; set; }
        public Integratejiraworkitems? IntegrateJiraWorkItems { get; set; }
        public Integrateboardsworkitems? IntegrateBoardsWorkItems { get; set; }
    }

    public class Definitioncreationsource
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class Integratejiraworkitems
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class Integrateboardsworkitems
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }
    public class Environment
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Rank { get; set; }
        public Owner? Owner { get; set; }
        public Variables? Variables { get; set; }
        public object[] VariableGroups { get; set; } = [];
        public Predeployapprovals? PreDeployApprovals { get; set; }
        public Deploystep? DeployStep { get; set; }
        public Postdeployapprovals? PostDeployApprovals { get; set; }
        public Deployphas[] DeployPhases { get; set; } = [];
        public Environmentoptions? EnvironmentOptions { get; set; }
        public object[] Demands { get; set; } = [];
        public Condition[] Conditions { get; set; } = [];
        public Executionpolicy? ExecutionPolicy { get; set; }
        public object[] Schedules { get; set; } = [];
        public Currentrelease? CurrentRelease { get; set; }
        public Retentionpolicy? RetentionPolicy { get; set; }
        public Properties1? Properties { get; set; }
        public Predeploymentgates? PreDeploymentGates { get; set; }
        public Postdeploymentgates? PostDeploymentGates { get; set; }
        public object[] EnvironmentTriggers { get; set; } = [];
        public string? BadgeUrl { get; set; }
    }

    public class Owner
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        public Links1? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Descriptor { get; set; }
    }

    public class Apppoolidentityusername
    {
        public string? Value { get; set; }
    }

    public class Predeployapprovals
    {
        public Approval[] Approvals { get; set; } = [];
        public Approvaloptions? ApprovalOptions { get; set; }
    }

    public class Approvaloptions
    {
        public object? RequiredApproverCount { get; set; }
        public bool ReleaseCreatorCanBeApprover { get; set; }
        public bool AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }
        public bool EnforceIdentityRevalidation { get; set; }
        public int TimeoutInMinutes { get; set; }
        public string? ExecutionOrder { get; set; }
    }

    public class Approval
    {
        public int Rank { get; set; }
        public bool IsAutomated { get; set; }
        public bool IsNotificationOn { get; set; }
        public int Id { get; set; }
        public Approver? Approver { get; set; }
    }

    public class Approver
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        public Links1? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsContainer { get; set; }
        public string? Descriptor { get; set; }
    }

    public class Deploystep
    {
        public int Id { get; set; }
    }

    public class Postdeployapprovals
    {
        public Approval[] Approvals { get; set; } = [];
        public Approvaloptions? ApprovalOptions { get; set; }
    }

    public class Environmentoptions
    {
        public string? EmailNotificationType { get; set; }
        public string? EmailRecipients { get; set; }
        public bool SkipArtifactsDownload { get; set; }
        public int TimeoutInMinutes { get; set; }
        public bool EnableAccessToken { get; set; }
        public bool PublishDeploymentStatus { get; set; }
        public bool BadgeEnabled { get; set; }
        public bool AutoLinkWorkItems { get; set; }
        public bool PullRequestDeploymentEnabled { get; set; }
    }

    public class Executionpolicy
    {
        public int ConcurrencyCount { get; set; }
        public int QueueDepthCount { get; set; }
    }

    public class Currentrelease
    {
        public int Id { get; set; }
        public string? Url { get; set; }
    }

    public class Retentionpolicy
    {
        public int DaysToKeep { get; set; }
        public int ReleasesToKeep { get; set; }
        public bool RetainBuild { get; set; }
    }

    public class Properties1
    {
        public Linkboardsworkitems? LinkBoardsWorkItems { get; set; }
        public Boardsenvironmenttype? BoardsEnvironmentType { get; set; }
    }

    public class Linkboardsworkitems
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class Boardsenvironmenttype
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class Predeploymentgates
    {
        public int Id { get; set; }
        public object? GatesOptions { get; set; }
        public object[] Gates { get; set; } = [];
    }

    public class Postdeploymentgates
    {
        public int Id { get; set; }
        public object? GatesOptions { get; set; }
        public object[] Gates { get; set; } = [];
    }

    public class Deployphas
    {
        public Deploymentinput? DeploymentInput { get; set; }
        public int Rank { get; set; }
        public string? PhaseType { get; set; }
        public string? Name { get; set; }
        public object? RefName { get; set; }
        public Workflowtask[] WorkflowTasks { get; set; } = [];
    }

    public class Deploymentinput
    {
        public Parallelexecution? ParallelExecution { get; set; }
        public Agentspecification? AgentSpecification { get; set; }
        public bool SkipArtifactsDownload { get; set; }
        public Artifactsdownloadinput? ArtifactsDownloadInput { get; set; }
        public int QueueId { get; set; }
        public object[] Demands { get; set; } = [];
        public bool EnableAccessToken { get; set; }
        public int TimeoutInMinutes { get; set; }
        public int JobCancelTimeoutInMinutes { get; set; }
        public string? Condition { get; set; }
    }

    public class Parallelexecution
    {
        public string? ParallelExecutionType { get; set; }
    }

    public class Agentspecification
    {
        public string? Identifier { get; set; }
    }

    public class Artifactsdownloadinput
    {
        public object[] DownloadInputs { get; set; } = [];
    }

    public class Workflowtask
    {
        public Environment? Environment { get; set; }
        public string? TaskId { get; set; }
        public string? Version { get; set; }
        public string? Name { get; set; }
        public string? RefName { get; set; }
        public bool Enabled { get; set; }
        public bool AlwaysRun { get; set; }
        public bool ContinueOnError { get; set; }
        public int TimeoutInMinutes { get; set; }
        public int RetryCountOnTaskFailure { get; set; }
        public string? DefinitionType { get; set; }
        public string? Condition { get; set; }
        public Inputs? Inputs { get; set; }
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
        public string? Lock { get; set; }
        public string? SRAutomationPAT { get; set; }
    }

    public class Condition
    {
        public string? Name { get; set; }
        public string? ConditionType { get; set; }
        public string? Value { get; set; }
        public object? Result { get; set; }
    }

    public class Artifact
    {
        public string? SourceId { get; set; }
        public string? Type { get; set; }
        public string? Alias { get; set; }
        public Definitionreference? DefinitionReference { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsRetained { get; set; }
    }

    public class Definitionreference
    {
        public Artifactsourcedefinitionurl? ArtifactSourceDefinitionUrl { get; set; }
        public Defaultversionbranch? DefaultVersionBranch { get; set; }
        public Defaultversionspecific? DefaultVersionSpecific { get; set; }
        public Defaultversiontags? DefaultVersionTags { get; set; }
        public Defaultversiontype? DefaultVersionType { get; set; }
        public Definition? Definition { get; set; }
        public Definitions? Definitions { get; set; }
        public Ismultidefinitiontype? IsMultiDefinitionType { get; set; }
        public Project? Project { get; set; }
        public Repository? Repository { get; set; }
        public Branches? Branches { get; set; }
        public Checkoutnestedsubmodules? CheckoutNestedSubmodules { get; set; }
        public Checkoutsubmodules? CheckoutSubmodules { get; set; }
        public Fetchdepth? FetchDepth { get; set; }
        public Gitlfssupport? GitLfsSupport { get; set; }
    }

    public class Artifactsourcedefinitionurl
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Defaultversionbranch
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Defaultversionspecific
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Defaultversiontags
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Defaultversiontype
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Definitions
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Ismultidefinitiontype
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Project
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Repository
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Branches
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Checkoutnestedsubmodules
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Checkoutsubmodules
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Fetchdepth
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Gitlfssupport
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
