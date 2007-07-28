Create table "cmsContent"
(
	"pk" Integer NOT NULL,
	"nodeId" Integer NOT NULL,
	"contentType" Integer NOT NULL,
 primary key ("pk")
) Without Oids;


Create table "cmsContentType"
(
	"pk" Integer NOT NULL,
	"nodeId" Integer NOT NULL,
	"alias" Varchar(255),
	"icon" Varchar(255),
 primary key ("pk")
) Without Oids;


Create table "cmsContentTypeAllowedContentType"
(
	"Id" Integer NOT NULL,
	"AllowedId" Integer NOT NULL,
 primary key ("Id","AllowedId")
) Without Oids;


Create table "cmsContentVersion"
(
	"id" Integer NOT NULL,
	"ContentId" Integer NOT NULL,
	"VersionId" Double precision NOT NULL,
	"VersionDate" Date NOT NULL Default (CURRENT_DATE)
) Without Oids;


Create table "cmsContentXml"
(
	"nodeId" Integer NOT NULL,
	"xml" Text NOT NULL,
 primary key ("nodeId")
) Without Oids;


Create table "cmsDataType"
(
	"pk" Integer NOT NULL,
	"nodeId" Integer NOT NULL,
	"controlId" Double precision NOT NULL,
	"dbType" Varchar(50) NOT NULL,
 primary key ("pk")
) Without Oids;


Create table "cmsDataTypePreValues"
(
	"id" Integer NOT NULL,
	"datatypeNodeId" Integer NOT NULL,
	"value" Varchar(255),
	"sortorder" Integer NOT NULL,
	"alias" Varchar(50),
 primary key ("id")
) Without Oids;


Create table "cmsDictionary"
(
	"pk" Integer NOT NULL,
	"id" Double precision NOT NULL,
	"parent" Double precision NOT NULL,
	"key" Varchar(1000) NOT NULL,
 primary key ("pk")
) Without Oids;


Create table "cmsDocument"
(
	"versionId" Double precision NOT NULL,
	"nodeId" Integer NOT NULL,
	"published" Bit(1) NOT NULL,
	"documentUser" Integer NOT NULL,
	"text" Varchar(255) NOT NULL,
	"releaseDate" Date,
	"expireDate" Date,
	"updateDate" Date NOT NULL Default (CURRENT_DATE),
	"templateId" Integer,
	"alias" Varchar(255),
	"newest" Bit(1) NOT NULL Default (B'0'),
 primary key ("versionId")
) Without Oids;


Create table "cmsDocumentType"
(
	"contentTypeNodeId" Integer NOT NULL,
	"templateNodeId" Integer NOT NULL,
	"IsDefault" Bit(1) NOT NULL Default (B'0'),
 primary key ("contentTypeNodeId","templateNodeId")
) Without Oids;


Create table "cmsLanguageText"
(
	"pk" Integer NOT NULL,
	"languageId" Integer NOT NULL,
	"UniqueId" Double precision NOT NULL,
	"value" Varchar(1000) NOT NULL,
 primary key ("pk")
) Without Oids;


Create table "cmsMacro"
(
	"id" Integer NOT NULL,
	"macroUseInEditor" Bit(1) NOT NULL Default (B'0'),
	"macroRefreshRate" Integer NOT NULL Default ((0)),
	"macroAlias" Varchar(50) NOT NULL,
	"macroName" Varchar(255),
	"macroScriptType" Varchar(255),
	"macroScriptAssembly" Varchar(255),
	"macroXSLT" Varchar(255),
	"macroCacheByPage" Bit(1) NOT NULL Default (B'1'),
	"macroCachePersonalized" Bit(1) NOT NULL Default (B'0'),
	"macroDontRender" Bit(1) NOT NULL Default (B'0'),
 primary key ("id")
) Without Oids;


Create table "cmsMacroProperty"
(
	"id" Integer NOT NULL,
	"macroPropertyHidden" Bit(1) NOT NULL Default (B'0'),
	"macroPropertyType" Smallint NOT NULL,
	"macro" Integer NOT NULL,
	"macroPropertySortOrder" Integer NOT NULL Default ((0)),
	"macroPropertyAlias" Varchar(50) NOT NULL,
	"macroPropertyName" Varchar(255) NOT NULL,
 primary key ("id")
) Without Oids;


Create table "cmsMacroPropertyType"
(
	"id" Smallint NOT NULL,
	"macroPropertyTypeAlias" Varchar(50),
	"macroPropertyTypeRenderAssembly" Varchar(255),
	"macroPropertyTypeRenderType" Varchar(255),
	"macroPropertyTypeBaseType" Varchar(255),
 primary key ("id")
) Without Oids;


Create table "cmsMember"
(
	"nodeId" Integer NOT NULL,
	"Email" Varchar(1000) NOT NULL Default (''),
	"LoginName" Varchar(1000) NOT NULL Default (''),
	"Password" Varchar(1000) NOT NULL Default ('')
) Without Oids;


Create table "cmsMember2MemberGroup"
(
	"Member" Integer NOT NULL,
	"MemberGroup" Integer NOT NULL,
 primary key ("Member","MemberGroup")
) Without Oids;


Create table "cmsMemberType"
(
	"pk" Integer NOT NULL,
	"NodeId" Integer NOT NULL,
	"propertytypeId" Integer NOT NULL,
	"memberCanEdit" Bit(1) NOT NULL Default (B'0'),
	"viewOnProfile" Bit(1) NOT NULL Default (B'0'),
 primary key ("pk")
) Without Oids;


Create table "cmsPropertyData"
(
	"id" Integer NOT NULL,
	"contentNodeId" Integer NOT NULL,
	"versionId" Double precision,
	"propertytypeid" Integer NOT NULL,
	"dataInt" Integer,
	"dataDate" Date,
	"dataNvarchar" Varchar(500),
	"dataNtext" Text,
 primary key ("id")
) Without Oids;


Create table "cmsPropertyType"
(
	"id" Integer NOT NULL,
	"dataTypeId" Integer NOT NULL,
	"contentTypeId" Integer NOT NULL,
	"tabId" Integer,
	"Alias" Varchar(255) NOT NULL,
	"Name" Varchar(255),
	"helpText" Varchar(1000),
	"sortOrder" Integer NOT NULL Default ((0)),
	"mandatory" Bit(1) NOT NULL Default (B'0'),
	"validationRegExp" Varchar(255),
	"Description" Varchar(2000),
 primary key ("id")
) Without Oids;


Create table "cmsStylesheet"
(
	"nodeId" Integer NOT NULL,
	"filename" Varchar(100) NOT NULL,
	"content" Text
) Without Oids;


Create table "cmsStylesheetProperty"
(
	"nodeId" Integer NOT NULL,
	"stylesheetPropertyEditor" Bit(1),
	"stylesheetPropertyAlias" Varchar(50),
	"stylesheetPropertyValue" Varchar(400)
) Without Oids;


Create table "cmsTab"
(
	"id" Integer NOT NULL,
	"contenttypeNodeId" Integer NOT NULL,
	"text" Varchar(255) NOT NULL,
	"sortorder" Integer NOT NULL,
 primary key ("id")
) Without Oids;


Create table "cmsTemplate"
(
	"pk" Integer NOT NULL,
	"nodeId" Integer NOT NULL,
	"master" Integer,
	"alias" Varchar(100),
	"design" Text NOT NULL,
 primary key ("pk")
) Without Oids;


Create table "umbracoApp"
(
	"appAlias" Varchar(50) NOT NULL,
	"sortOrder" Integer NOT NULL Default ((0)),
	"appIcon" Varchar(255) NOT NULL,
	"appName" Varchar(255) NOT NULL,
	"appInitWithTreeAlias" Varchar(255),
 primary key ("appAlias")
) Without Oids;


Create table "umbracoAppTree"
(
	"appAlias" Varchar(50) NOT NULL,
	"treeAlias" Varchar(150) NOT NULL,
	"treeSilent" Bit(1) NOT NULL Default (B'0'),
	"treeInitialize" Bit(1) NOT NULL Default (B'1'),
	"treeSortOrder" Integer NOT NULL,
	"treeTitle" Varchar(255) NOT NULL,
	"treeIconClosed" Varchar(255) NOT NULL,
	"treeIconOpen" Varchar(255) NOT NULL,
	"treeHandlerAssembly" Varchar(255) NOT NULL,
	"treeHandlerType" Varchar(255) NOT NULL,
 primary key ("appAlias","treeAlias")
) Without Oids;


Create table "umbracoDomains"
(
	"id" Integer NOT NULL,
	"domainDefaultLanguage" Integer,
	"domainRootStructureID" Integer,
	"domainName" Varchar(255) NOT NULL,
 primary key ("id")
) Without Oids;


Create table "umbracoLanguage"
(
	"id" Smallint NOT NULL,
	"languageISOCode" Varchar(5),
	"languageCultureName" Varchar(20),
 primary key ("id")
) Without Oids;


Create table "umbracoLog"
(
	"id" Integer NOT NULL,
	"userId" Integer NOT NULL,
	"NodeId" Integer NOT NULL,
	"Datestamp" Date NOT NULL Default (CURRENT_DATE),
	"logHeader" Varchar(50) NOT NULL,
	"logComment" Varchar(1000),
 primary key ("id")
) Without Oids;


Create table "umbracoNode"
(
	"id" Integer NOT NULL,
	"trashed" Bit(1) NOT NULL Default (B'0'),
	"parentID" Integer NOT NULL,
	"nodeUser" Integer,
	"level" Smallint NOT NULL,
	"path" Varchar(150) NOT NULL,
	"sortOrder" Integer NOT NULL,
	"uniqueID" Double precision,
	"text" Varchar(255),
	"nodeObjectType" Double precision,
	"createDate" Date NOT NULL Default (CURRENT_DATE),
 primary key ("id")
) Without Oids;


Create table "umbracoRelation"
(
	"id" Integer NOT NULL,
	"parentId" Integer NOT NULL,
	"childId" Integer NOT NULL,
	"relType" Integer NOT NULL,
	"datetime" Date NOT NULL Default (CURRENT_DATE),
	"comment" Varchar(1000) NOT NULL,
 primary key ("id")
) Without Oids;


Create table "umbracoRelationType"
(
	"id" Integer NOT NULL,
	"dual" Bit(1) NOT NULL,
	"parentObjectType" Double precision NOT NULL,
	"childObjectType" Double precision NOT NULL,
	"name" Varchar(255) NOT NULL,
	"alias" Varchar(100),
 primary key ("id")
) Without Oids;


Create table "umbracoStatEntry"
(
	"SessionId" Integer NOT NULL,
	"EntryTime" Date NOT NULL,
	"RefNodeId" Integer NOT NULL,
	"NodeId" Integer NOT NULL,
 primary key ("SessionId","EntryTime","RefNodeId","NodeId")
) Without Oids;


Create table "umbracoStatSession"
(
	"id" Integer NOT NULL,
	"MemberId" Double precision,
	"NewsletterId" Integer,
	"ReturningUser" Bit(1) NOT NULL,
	"SessionStart" Date NOT NULL,
	"SessionEnd" Date,
	"Language" Varchar(20) NOT NULL,
	"UserAgent" Varchar(255) NOT NULL,
	"Browser" Varchar(255) NOT NULL,
	"BrowserVersion" Varchar(20) NOT NULL,
	"OperatingSystem" Varchar(50) NOT NULL,
	"Ip" Varchar(50) NOT NULL,
	"Referrer" Varchar(255) NOT NULL,
	"ReferrerKeyword" Varchar(255) NOT NULL,
	"allowCookies" Bit(1) NOT NULL Default (B'0'),
	"visitorId" Char(36),
	"browserType" Varchar(255),
	"isHuman" Bit(1) Default (B'0'),
 primary key ("id")
) Without Oids;


Create table "umbracoStylesheet"
(
	"nodeId" Integer NOT NULL,
	"filename" Varchar(100) NOT NULL,
	"content" Text,
 primary key ("nodeId")
) Without Oids;


Create table "umbracoStylesheetProperty"
(
	"id" Smallint NOT NULL,
	"stylesheetPropertyEditor" Bit(1) NOT NULL Default (B'0'),
	"stylesheet" Integer NOT NULL,
	"stylesheetPropertyAlias" Varchar(50),
	"stylesheetPropertyName" Varchar(255),
	"stylesheetPropertyValue" Varchar(400),
 primary key ("id")
) Without Oids;


Create table "umbracoUser"
(
	"id" Integer NOT NULL,
	"userDisabled" Bit(1) NOT NULL Default (B'0'),
	"userNoConsole" Bit(1) NOT NULL Default (B'0'),
	"userType" Smallint NOT NULL,
	"startStructureID" Integer NOT NULL,
	"startMediaID" Integer,
	"userName" Varchar(255) NOT NULL,
	"userLogin" Varchar(125) NOT NULL,
	"userPassword" Varchar(125) NOT NULL,
	"userEmail" Varchar(255) NOT NULL,
	"userDefaultPermissions" Varchar(50),
	"userLanguage" Varchar(10),
 primary key ("id")
) Without Oids;


Create table "umbracoUser2app"
(
	"user" Integer NOT NULL,
	"app" Varchar(50) NOT NULL,
 primary key ("user","app")
) Without Oids;


Create table "umbracoUser2NodeNotify"
(
	"userId" Integer NOT NULL,
	"nodeId" Integer NOT NULL,
	"action" Char(1) NOT NULL,
 primary key ("userId","nodeId","action")
) Without Oids;


Create table "umbracoUser2NodePermission"
(
	"userId" Integer NOT NULL,
	"nodeId" Integer NOT NULL,
	"permission" Char(1) NOT NULL,
 primary key ("userId","nodeId","permission")
) Without Oids;


Create table "umbracoUser2userGroup"
(
	"user" Integer NOT NULL,
	"userGroup" Smallint NOT NULL,
 primary key ("user","userGroup")
) Without Oids;


Create table "umbracoUserGroup"
(
	"id" Smallint NOT NULL,
	"userGroupName" Varchar(255) NOT NULL,
 primary key ("id")
) Without Oids;


Create table "umbracoUserLogins"
(
	"contextID" Double precision NOT NULL,
	"userID" Integer NOT NULL,
	"timeout" Bigint NOT NULL
) Without Oids;


Create table "umbracoUserType"
(
	"id" Smallint NOT NULL,
	"userTypeAlias" Varchar(50),
	"userTypeName" Varchar(255) NOT NULL,
	"userTypeDefaultPermissions" Varchar(50),
 primary key ("id")
) Without Oids;


Create index "IX_cmsPropertyData" on "cmsPropertyData" using btree ("id");
Create index "IX_cmsPropertyData_1" on "cmsPropertyData" using btree ("contentNodeId");
Create index "IX_cmsPropertyData_2" on "cmsPropertyData" using btree ("versionId");
Create index "IX_cmsPropertyData_3" on "cmsPropertyData" using btree ("propertytypeid");
Create index "IX_umbracoNodeParentId" on "umbracoNode" using btree ("parentID");
Create index "IX_umbracoNodeObjectType" on "umbracoNode" using btree ("nodeObjectType");


Alter table "cmsMacroProperty" add  foreign key ("macroPropertyType") references "cmsMacroPropertyType" ("id") on update restrict on delete restrict;

Alter table "cmsPropertyType" add  foreign key ("tabId") references "cmsTab" ("id") on update restrict on delete restrict;

Alter table "umbracoAppTree" add  foreign key ("appAlias") references "umbracoApp" ("appAlias") on update restrict on delete restrict;

Alter table "umbracoUser2app" add  foreign key ("app") references "umbracoApp" ("appAlias") on update restrict on delete restrict;

Alter table "cmsContent" add  foreign key ("nodeId") references "umbracoNode" ("id") on update restrict on delete restrict;

Alter table "cmsContentType" add  foreign key ("nodeId") references "umbracoNode" ("id") on update restrict on delete restrict;

Alter table "cmsDocument" add  foreign key ("nodeId") references "umbracoNode" ("id") on update restrict on delete restrict;

Alter table "cmsPropertyData" add  foreign key ("contentNodeId") references "umbracoNode" ("id") on update restrict on delete restrict;

Alter table "cmsTemplate" add  foreign key ("nodeId") references "umbracoNode" ("id") on update restrict on delete restrict;

Alter table "umbracoNode" add  foreign key ("parentID") references "umbracoNode" ("id") on update restrict on delete restrict;

Alter table "umbracoUser2app" add  foreign key ("user") references "umbracoUser" ("id") on update restrict on delete restrict;

Alter table "umbracoUser2userGroup" add  foreign key ("user") references "umbracoUser" ("id") on update restrict on delete restrict;

Alter table "umbracoUser2userGroup" add  foreign key ("userGroup") references "umbracoUserGroup" ("id") on update restrict on delete restrict;

Alter table "umbracoUser" add  foreign key ("userType") references "umbracoUserType" ("id") on update restrict on delete restrict;
