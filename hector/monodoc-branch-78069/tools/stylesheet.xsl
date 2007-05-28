<?xml version="1.0"?>

<!--
	Based on Mono's /monodoc/browser/mono-ecma.xsl file.
-->

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	>
	
	<xsl:output omit-xml-declaration="yes" />

	<!-- TEMPLATE PARAMETERS -->

	<xsl:param name="language" select="'C#'"/>
	<xsl:param name="ext" select="'xml'"/>
	<xsl:param name="basepath" select="'./'"/>
	
	<xsl:variable name="Index" select="document('../index.xml', .)"/>
	<xsl:variable name="ThisType" select="/Type"/>

	<!-- The namespace that the current type belongs to. -->
	<xsl:variable name="TypeNamespace" select="substring(/Type/@FullName, 1, string-length(/Type/@FullName) - string-length(/Type/@Name) - 1)"/>		
	<xsl:variable name="mono-docs">http://www.go-mono.com/docs/monodoc.ashx?link=</xsl:variable>

	<!-- THE MAIN RENDERING TEMPLATE -->

	<xsl:template match="Type">
		<Page>
		
		<Title>
			<xsl:value-of select="translate (@FullName, '+', '.')" />
		</Title>
		
		<CollectionTitle>
			<a href="{$basepath}index.{$ext}"><xsl:value-of select="AssemblyInfo/AssemblyName"/></a>
			:
			<a href="index.{$ext}"><xsl:value-of select="$TypeNamespace"/> Namespace</a>
		</CollectionTitle>
		
		<PageTitle>
			<xsl:value-of select="translate (@Name, '+', '.')"/>
			<xsl:text xml:space="preserve"> </xsl:text>
			<xsl:if test="count(Docs/typeparam) &gt; 0">Generic</xsl:if>
			<xsl:text xml:space="preserve"> </xsl:text>
			<xsl:call-template name="GetTypeDescription" />
		</PageTitle>
		
		<!--
		<SideBar>
			<p style="font-weight: bold; border-bottom: thin solid black"><a href="index.{$ext}"><xsl:value-of select="$TypeNamespace"/></a></p>

			<xsl:for-each select="document('index.xml',.)/Overview/Types/Namespace[@Name=$TypeNamespace]/Type">
				<xsl:sort select="@Name"/>
				<div>
					<a href="../{parent::Namespace/@Name}/{@Name}.{$ext}">
						<xsl:value-of select="@Name"/>
					</a>
				</div>
			</xsl:for-each>
		</SideBar>
		-->

		<!-- TYPE OVERVIEW -->
		
		<Summary>
			<!-- summary -->
			<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
		</Summary>

		<Signature>
			<!-- signature -->
			<table class="SignatureTable" cellspacing="0" width="100%">
			<tr><td>
				<table class="InnerSignatureTable" cellpadding="10" cellspacing="0" width="100%">
				<tr><td>
					<xsl:choose>
					<xsl:when test="$language='C#'">

						<xsl:for-each select="Attributes/Attribute">
							<div>[<xsl:value-of select="AttributeName"/>]</div>
						</xsl:for-each>
	
						<xsl:choose>

						<xsl:when test="Base/BaseTypeName='System.Enum'">
							<xsl:call-template name="getmodifiers">
								<xsl:with-param name="sig" select="TypeSignature[@Language='C#']/@Value"/>
							</xsl:call-template>

							enum
	
							<!-- member name, argument list -->
							<b>
							<xsl:value-of select="translate (@Name, '+', '.')"/>
							</b>
						</xsl:when>
	
						<xsl:when test="Base/BaseTypeName='System.Delegate' or Base/BaseTypeName='System.MulticastDelegate'">
							<xsl:choose>

							<xsl:when test="count(Parameters) &gt; 0 and count(ReturnValue) &gt; 0">
							<!-- Only recreate the delegate signature if the appropriate information
								is present in the XML file. -->

							<xsl:call-template name="getmodifiers">
								<xsl:with-param name="sig" select="TypeSignature[@Language='C#']/@Value"/>
							</xsl:call-template>

							delegate
	
							<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
	
							<!-- hard space -->
							<xsl:value-of select="' '"/>
	
							<!-- member name, argument list -->
							<b>
							<xsl:value-of select="translate (@Name, '+', '.')"/>
							</b>

							<!-- hard space -->
							<xsl:value-of select="' '"/>

							<xsl:value-of select="'('"/> <!-- prevents whitespace issues -->
							
							<xsl:variable name="HasManyArgs" select="count(Parameters/Parameter) &gt; 4"/>

							<xsl:for-each select="Parameters/Parameter">
								<xsl:call-template name="ShowParameter">
									<xsl:with-param name="Param" select="."/>
									<xsl:with-param name="TypeNamespace" select="$TypeNamespace"/>
								</xsl:call-template>

								<xsl:if test="not(position()=last())">, </xsl:if>
							</xsl:for-each>
							
							<xsl:value-of select="')'"/>

							</xsl:when>
							
							<xsl:otherwise>
								<xsl:apply-templates select="TypeSignature[@Language=$language]/@Value"/>	
							</xsl:otherwise>

							</xsl:choose>

							
						</xsl:when>

						<xsl:otherwise>
							<xsl:call-template name="getmodifiers">
								<xsl:with-param name="sig" select="TypeSignature[@Language='C#']/@Value"/>
								<xsl:with-param name="typetype" select="true()"/>
							</xsl:call-template>
		
							<xsl:value-of select="' '"/>
		
							<b><xsl:value-of select="translate (@Name, '+', '.')"/></b>
		
							<xsl:variable name="HasStandardBaseType" select="Base/BaseTypeName='System.Object' or Base/BaseTypeName='System.ValueType'"/>
							<xsl:variable name="HasBaseType" select="count(Base/BaseTypeName)>0"/>
							<xsl:if test="(($HasBaseType) and not($HasStandardBaseType)) or not(count(Interfaces/Interface)=0)"> :
		
								<xsl:if test="not($HasStandardBaseType)">
									<xsl:apply-templates select="Base/BaseTypeName" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
									<xsl:if test="not(count(Interfaces/Interface)=0)">,	</xsl:if>
								</xsl:if>
		
								<xsl:for-each select="Interfaces/Interface">
									<xsl:if test="not(position()=1)">, </xsl:if>
									<xsl:apply-templates select="InterfaceName" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
								</xsl:for-each>
							
							</xsl:if>
						</xsl:otherwise>

						</xsl:choose>

					</xsl:when>

					<xsl:otherwise>
						<xsl:apply-templates select="TypeSignature[@Language=$language]/@Value"/>
					</xsl:otherwise>
					
					</xsl:choose>
				</td></tr>
				</table>
			</td></tr>
			</table>

			<br/>
		</Signature>
			
		<Remarks>
			<xsl:call-template name="DisplayDocsInformation"/>

			<!-- MEMBER LISTING -->
			<xsl:if test="not(Base/BaseTypeName='System.Delegate' or Base/BaseTypeName='System.MulticastDelegate' or Base/BaseTypeName='System.Enum')">
				<h2 class="Section">Members</h2>
	
				<div class="SectionBox">
				
				<xsl:if test="Base/BaseTypeName">
					<p>
						See Also: Inherited members from
						<xsl:apply-templates select="Base/BaseTypeName" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>.
					</p>
				</xsl:if>

				<!-- list each type of member (public, then protected) -->

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Constructor'"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Constructor'"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Field'"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Field'"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Property'"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Property'"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Method'"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Method'"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Event'"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Event'"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="'Operator'"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>
				
			</div>
			</xsl:if>
			
		</Remarks>
			
		<Members>
		<!-- MEMBER DETAILS -->
			<xsl:if test="not(Base/BaseTypeName='System.Delegate' or Base/BaseTypeName='System.MulticastDelegate' or Base/BaseTypeName='System.Enum')">
			<xsl:variable name="Type" select="."/>
			
			<h2 class="Section">Member Details</h2>
			
			<div class="SectionBox">

			<xsl:for-each select="Members/Member">
			
				<xsl:variable name="linkid">
					<xsl:call-template name="GetLinkId">
						<xsl:with-param name="type" select="../.." />
						<xsl:with-param name="member" select="." />
					</xsl:call-template>
				</xsl:variable>

				<h3 id="{$linkid}" class="MemberName">
					<xsl:choose>
						<xsl:when test="MemberType='Constructor'">
							<xsl:call-template name="GetConstructorName">
								<xsl:with-param name="type" select="../.." />
								<xsl:with-param name="ctor" select="." />
							</xsl:call-template>
						</xsl:when>
						<xsl:when test="@MemberName='op_Implicit' or @MemberName='op_Explicit'">
							<xsl:text>Conversion</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@MemberName"/>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text xml:space="preserve"> </xsl:text>
					<xsl:if test="count(Docs/typeparam) &gt; 0">
						<xsl:text>Generic </xsl:text>
					</xsl:if>
					<xsl:value-of select="MemberType" />
				</h3>
					
				<div class="MemberSignature">
					<!-- recreate the signature -->

					<xsl:call-template name="getmodifiers">
						<xsl:with-param name="sig" select="MemberSignature[@Language='C#']/@Value"/>
					</xsl:call-template>

					<xsl:if test="MemberType = 'Event'">
						<xsl:text>event </xsl:text>

						<xsl:if test="ReturnValue/ReturnType=''">
							<xsl:value-of select="substring-before(substring-after(MemberSignature[@Language='C#']/@Value, 'event '), concat(' ', @MemberName))"/>
						</xsl:if>
					</xsl:if>

					<!-- return value (comes out "" where not applicable/available) -->
					<xsl:choose>
					<xsl:when test="@MemberName='op_Implicit'">
						<xsl:text>implicit operator</xsl:text>
					</xsl:when>
					<xsl:when test="@MemberName='op_Explicit'">
						<xsl:text>explicit operator</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink">
							<xsl:with-param name="wrt" select="$TypeNamespace"/>
						</xsl:apply-templates>
					</xsl:otherwise>					
					</xsl:choose>

					<!-- hard space -->
					<xsl:value-of select="' '"/>

					<!-- member name -->
					<xsl:choose>
					
					<!-- Constructors get the name of the class -->
					<xsl:when test="MemberType='Constructor'">
						<b>
							<xsl:call-template name="GetConstructorName">
								<xsl:with-param name="type" select="../.." />
								<xsl:with-param name="ctor" select="." />
							</xsl:call-template>
						</b>
					</xsl:when>
					
					<!-- Conversion operators get the return type -->
					<xsl:when test="@MemberName='op_Implicit' or @MemberName='op_Explicit'">
						<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink">
							<xsl:with-param name="wrt" select="$TypeNamespace"/>
						</xsl:apply-templates>
					</xsl:when>
					
					<!-- Regular operators get their symbol -->
					<xsl:when test="@MemberName='op_UnaryPlus'">+</xsl:when>
					<xsl:when test="@MemberName='op_UnaryNegation'">-</xsl:when>
					<xsl:when test="@MemberName='op_LogicalNot'">!</xsl:when>
					<xsl:when test="@MemberName='op_OnesComplement'">~</xsl:when>
					<xsl:when test="@MemberName='op_Increment'">++</xsl:when>
					<xsl:when test="@MemberName='op_Decrement'">--</xsl:when>
					<xsl:when test="@MemberName='op_True'">true</xsl:when>
					<xsl:when test="@MemberName='op_False'">false</xsl:when>
					<xsl:when test="@MemberName='op_Addition'">+</xsl:when>
					<xsl:when test="@MemberName='op_Subtraction'">-</xsl:when>
					<xsl:when test="@MemberName='op_Multiply'">*</xsl:when>
					<xsl:when test="@MemberName='op_Division'">/</xsl:when>
					<xsl:when test="@MemberName='op_Modulus'">%</xsl:when>
					<xsl:when test="@MemberName='op_BitwiseAnd'">&amp;</xsl:when>
					<xsl:when test="@MemberName='op_BitwiseOr'">|</xsl:when>
					<xsl:when test="@MemberName='op_ExclusiveOr'">^</xsl:when>
					<xsl:when test="@MemberName='op_LeftShift'">&lt;&lt;</xsl:when>
					<xsl:when test="@MemberName='op_RightShift'">&gt;&gt;</xsl:when>
					<xsl:when test="@MemberName='op_Equality'">==</xsl:when>
					<xsl:when test="@MemberName='op_Inequality'">!=</xsl:when>
					<xsl:when test="@MemberName='op_GreaterThan'">&gt;</xsl:when>
					<xsl:when test="@MemberName='op_LessThan'">&lt;</xsl:when>
					<xsl:when test="@MemberName='op_GreaterThanOrEqual'">&gt;=</xsl:when>
					<xsl:when test="@MemberName='op_LessThanOrEqual'">&lt;=</xsl:when>

					<xsl:when test="MemberType='Property' and count(Parameters/Parameter) &gt; 0">
						<!-- C# only permits indexer properties to have arguments -->
						<xsl:text>this</xsl:text>
					</xsl:when>
					
					<!-- Everything else just gets its name -->
					<xsl:otherwise>
						<b><xsl:value-of select="@MemberName"/></b>
					</xsl:otherwise>
					</xsl:choose>

					<!-- hard space -->
					<xsl:value-of select="' '"/>

					<!-- argument list -->
					<xsl:if test="MemberType='Method' or MemberType='Constructor' or (MemberType='Property' and count(Parameters/Parameter))">
						<xsl:if test="not(MemberType='Property')">(</xsl:if>
						<xsl:if test="MemberType='Property'">[</xsl:if>

						<xsl:variable name="HasManyArgs" select="count(Parameters/Parameter) &gt; 4"/>

						<xsl:for-each select="Parameters/Parameter">
							<xsl:call-template name="ShowParameter">
								<xsl:with-param name="Param" select="."/>
								<xsl:with-param name="TypeNamespace" select="$TypeNamespace"/>
							</xsl:call-template>

							<xsl:if test="not(position()=last())">, </xsl:if>
						</xsl:for-each>
						<xsl:if test="not(MemberType='Property')">)</xsl:if>
						<xsl:if test="MemberType='Property'">]</xsl:if>
					</xsl:if>

					<xsl:if test="MemberType='Property'">
						<xsl:value-of select="' '"/>
						<xsl:text>{</xsl:text>
						<xsl:value-of select="substring-before(substring-after(MemberSignature[@Language='C#']/@Value, '{'), '}')"/>
						<xsl:text>}</xsl:text>
					</xsl:if>
				</div>
				
				<div class="MemberBox">

				<!-- summary -->

				<xsl:if test="contains(MemberSignature[@Language='C#']/@Value,'this[')">
					<p><i>This is the default property for this class.</i></p>
				</xsl:if>
				
				<!-- member value -->

				<xsl:if test="MemberValue">
				<p><b>Value: </b>
					<xsl:value-of select="MemberValue"/>
				</p>
				</xsl:if>

				<p>
					<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
				</p>

				<xsl:if test="Attributes/Attribute">
					<p>
					<xsl:text>Attributes:</xsl:text>
					<xsl:for-each select="Attributes/Attribute">
						<xsl:if test="not(position()=1)">, </xsl:if>
						<xsl:value-of select="AttributeName"/>
					</xsl:for-each>
					</p>
				</xsl:if>

				<xsl:call-template name="DisplayDocsInformation"/>

				<hr size="1"/>
					
				</div>

			</xsl:for-each>
			
			</div>
			</xsl:if>
			
			</Members>
			
			<Copyright>
			</Copyright>
			
		</Page>
	</xsl:template>

	<xsl:template name="GetConstructorName">
		<xsl:param name="type" />
		<xsl:param name="ctor" />

		<xsl:choose>
			<xsl:when test="contains($type/@Name, '&lt;')">
				<xsl:value-of select="translate (substring-before ($type/@Name, '&lt;'), '+', '.')" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="translate ($type/@Name, '+', '.')" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="ShowParameter">
		<xsl:param name="Param"/>
		<xsl:param name="TypeNamespace"/>
		<xsl:param name="prototype" select="false()"/>

		<xsl:if test="not($prototype)">
			<xsl:for-each select="$Param/Attributes/Attribute[not(Exclude='1') and not(AttributeName='ParamArrayAttribute' or AttributeName='System.ParamArray')]">
				<xsl:text>[</xsl:text>
				<xsl:value-of select="AttributeName"/>
				<xsl:text>]</xsl:text>
				<xsl:value-of select="' '"/>
			</xsl:for-each>
		</xsl:if>

		<xsl:if test="count($Param/Attributes/Attribute/AttributeName[.='ParamArrayAttribute' or .='System.ParamArray'])">
			<b>params</b>
			<xsl:value-of select="' '"/>
		</xsl:if>

		<xsl:if test="$Param/@RefType">
			<i><xsl:value-of select="$Param/@RefType"/></i>
			<!-- hard space -->
			<xsl:value-of select="' '"/>
		</xsl:if>

		<!-- parameter type link -->
		<xsl:apply-templates select="$Param/@Type" mode="typelink">
			<xsl:with-param name="wrt" select="$TypeNamespace"/>
		</xsl:apply-templates>

		<xsl:if test="not($prototype)">
			<!-- hard space -->
			<xsl:value-of select="' '"/>
	
			<!-- parameter name -->
			<xsl:value-of select="$Param/@Name"/>
		</xsl:if>
	</xsl:template>

	<xsl:template name="DisplayDocsInformation">
		<!-- The namespace that the current type belongs to. -->
		<xsl:variable name="TypeNamespace" select="substring(@FullName, 1, string-length(@FullName) - string-length(@Name) - 1)"/>

		<!-- alt member: not sure what these are for, actually -->

		<xsl:if test="count(Docs/altmember)">
			<h4 class="Subsection">See Also</h4>
			<div class="SubsectionBox">
			<xsl:for-each select="Docs/altmember">
				<div><xsl:apply-templates select="@cref" mode="cref"/></div>
			</xsl:for-each>
			</div>
		</xsl:if>

		<!-- parameters & return & value -->

		<xsl:if test="count(Docs/typeparam)">
			<h4 class="Subsection">Type Parameters</h4>
			<div class="SubsectionBox">
			<dl>
			<xsl:for-each select="Docs/typeparam">
				<dt><i><xsl:value-of select="@name"/></i></dt>
				<dd>
					<xsl:apply-templates select="." mode="notoppara"/>
				</dd>
			</xsl:for-each>
			</dl>
			</div>
		</xsl:if>
		<xsl:if test="count(Docs/param)">
			<h4 class="Subsection">Parameters</h4>
			<div class="SubsectionBox">
			<dl>
			<xsl:for-each select="Docs/param">
				<dt><i><xsl:value-of select="@name"/></i></dt>
				<dd>
					<xsl:apply-templates select="." mode="notoppara"/>
				</dd>
			</xsl:for-each>
			</dl>
			</div>
		</xsl:if>
		<xsl:if test="count(Docs/returns)">
			<h4 class="Subsection">Returns</h4>
			<div class="SubsectionBox">
				<xsl:apply-templates select="Docs/returns" mode="notoppara"/>
			</div>
		</xsl:if>
		<xsl:if test="count(Docs/value)">
			<h4 class="Subsection">Value</h4>
			<div class="SubsectionBox">
				<xsl:apply-templates select="Docs/value" mode="notoppara"/>
			</div>
		</xsl:if>

		<!-- thread safety -->

		<xsl:if test="count(ThreadingSafetyStatement)">
			<h4 class="Subsection">Thread Safety</h4>
			<div class="SubsectionBox">
			<xsl:apply-templates select="ThreadingSafetyStatement" mode="notoppara"/>
			</div>
		</xsl:if>


		<!-- permissions -->

		<xsl:if test="count(Docs/permission)">
			<h4>Permissions</h4>
			<div class="SubsectionBox">
			<table class="TypePermissionsTable" border="1" cellpadding="6">
			<tr><th>Type</th><th>Reason</th></tr>
			<xsl:for-each select="Docs/permission">
				<tr valign="top">
				<td>
					<xsl:apply-templates select="@cref" mode="typelink">
						<xsl:with-param name="wrt" select="$TypeNamespace"/>
					</xsl:apply-templates>
				</td>
				<td>
					<xsl:apply-templates select="." mode="notoppara"/>
				</td>
				</tr>
			</xsl:for-each>
			</table>
			</div>
		</xsl:if>

		<!-- method/property/constructor exceptions -->

		<xsl:if test="count(Docs/exception)">
			<h4 class="Subsection">Exceptions</h4>
			<div class="SubsectionBox">
			<table class="ExceptionsTable">
			<tr><th>Type</th><th>Condition</th></tr>
			<xsl:for-each select="Docs/exception">
				<tr valign="top">
				<td>
					<xsl:apply-templates select="@cref" mode="typelink">
						<xsl:with-param name="wrt" select="$TypeNamespace"/>
					</xsl:apply-templates>
				</td>
				<td>
					<xsl:apply-templates select="." mode="notoppara"/>
				</td>
				</tr>
			</xsl:for-each>
			</table>
			</div>
		</xsl:if>

		<!-- remarks -->

		<xsl:if test="count(Docs/remarks)">
			<h4 class="Subsection">Remarks</h4>
			<div class="SubsectionBox">
			<xsl:apply-templates select="Docs/remarks" mode="notoppara"/>
			</div>
		</xsl:if>

		<!-- enumeration values -->

		<xsl:if test="Base/BaseTypeName = 'System.Enum'">
			<div class="Subsection">Members</div>
			<div class="SubsectionBox">
			<table class="EnumerationsTable">
			<tr>
				<th>Member name</th>
				<th>Description</th>
			</tr>

			<xsl:for-each select="Members/Member[MemberType='Field']">
				<xsl:if test="not(@MemberName='value__')">
					<tr><td><b>
						<xsl:value-of select="@MemberName"/>
					</b></td>
					<td>
						<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
					</td>
					</tr>
				</xsl:if>
			</xsl:for-each>

			</table>
			</div>
		</xsl:if>

		<!-- examples -->

		<xsl:if test="count(Docs/example)">
			<h4>Examples</h4>
			<div class="SubsectionBox">
			<xsl:for-each select="Docs/example">
				<xsl:apply-templates select="." mode="notoppara"/>
			</xsl:for-each>
			</div>
		</xsl:if>
	</xsl:template>

	
	<!-- Transforms the contents of the selected node into a hyperlink to the type named by the node.  The node can contain a type name (eg System.Object) or a type link (eg T:System.String). Use wrt parameter to specify the current namespace. -->

	<xsl:template match="*|@*" mode="typelink">
		<xsl:param name="wrt" select="'notset'"/>
		
		<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="."/>
				<xsl:with-param name="wrt" select="$wrt"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="maketypelink">
		<xsl:param name="type"/>
		<xsl:param name="wrt" select="'notset'"/>
		
		<xsl:choose>

		<!-- chop off T: -->
		<xsl:when test="starts-with($type, 'T:')">
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="substring($type, 3)"/>
				<xsl:with-param name="wrt" select="$wrt"/>
			</xsl:call-template>
		</xsl:when>

		<xsl:when test="contains($type, '&amp;')">
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="substring($type, 1, string-length($type)-1)"/>
				<xsl:with-param name="wrt" select="$wrt"/>
			</xsl:call-template>
		</xsl:when>

		<xsl:when test="contains($type, '[]')">
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="substring($type, 1, string-length($type)-2)"/>
				<xsl:with-param name="wrt" select="$wrt"/>
			</xsl:call-template>
			<xsl:value-of select="'[]'"/>
		</xsl:when>

		<xsl:when test="contains($type, '*')">
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="substring($type, 1, string-length($type)-1)"/>
				<xsl:with-param name="wrt" select="$wrt"/>
			</xsl:call-template>
			<xsl:value-of select="'*'"/>
		</xsl:when>
		
		<!-- if this is a generic type parameter, don't make a link but italicize it and give it a tooltip instead -->
		<xsl:when test="count($ThisType/TypeParameters/*[.=$type] | ancestor::Member/Docs/typeparam[@name=$type]) = 1">
			<!-- note that we check if it is a generic type using /Type/TypeParameters because that will have type parameters declared in an outer class if this is a nested class, but then we get the tooltip text from the type parameters documented in this file -->
			<i title="{$ThisType/Docs/typeparam[@name=$type] | ancestor::Member/Docs/typeparam[@name=$type]}"><xsl:value-of select="$type"/></i>
		</xsl:when>
		
		<!-- if this is a generic type parameter of a base type, replace it with the type that it was instantiated with -->
		<xsl:when test="count(ancestor::Members/BaseTypeArgument[@TypeParamName=$type]) = 1">
			<!-- note that an overridden type parameter may be referenced in a type parameter within $type, but we can't replace that nicely since we can't parse generic type names here -->
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="ancestor::Members/BaseTypeArgument[@TypeParamName=$type]"/>
				<xsl:with-param name="wrt" select="$wrt"/>
			</xsl:call-template>
		</xsl:when>
		

		<xsl:otherwise>
			<xsl:variable name="escaped-type">
				<xsl:call-template name="GetEscapedTypeName">
					<xsl:with-param name="typename" select="$type" />
				</xsl:call-template>
			</xsl:variable>
			<xsl:variable name="T" select="$type"/>
			<xsl:variable name="typeentry" select="$Index/Overview/Types/Namespace/Type[concat(parent::Namespace/@Name,'.',@Name) = $T]" />
			<xsl:for-each select="$Index/Overview/Types/Namespace/Type[concat(parent::Namespace/@Name, '.', @Name)]">
			</xsl:for-each>
			<a>
				<xsl:attribute name="href">
					<xsl:call-template name="GetLinkTarget">
						<xsl:with-param name="type" select="$escaped-type" />
						<xsl:with-param name="local-suffix" />
						<xsl:with-param name="remote">
							<xsl:value-of select="$mono-docs" />
							<xsl:text>T:</xsl:text>
							<xsl:value-of select="$escaped-type" />
						</xsl:with-param>
					</xsl:call-template>
				</xsl:attribute>
	
				<xsl:call-template name="GetTypeDisplayName">
					<xsl:with-param name="T" select="$T"/>
					<xsl:with-param name="wrt" select="$wrt"/>
				</xsl:call-template>
			</a>
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetTypeDisplayName">
		<xsl:param name="T"/>
		<xsl:param name="wrt"/>
		
				<!-- use C#-style names -->
				<xsl:choose>
					<xsl:when test="$T='System.Object'">object</xsl:when>
					<xsl:when test="$T='System.Boolean'">bool</xsl:when>
					<xsl:when test="$T='System.Byte'">byte</xsl:when>
					<xsl:when test="$T='System.Char'">char</xsl:when>
					<xsl:when test="$T='System.Decimal'">decimal</xsl:when>
					<xsl:when test="$T='System.Double'">double</xsl:when>
					<xsl:when test="$T='System.Int16'">short</xsl:when>
					<xsl:when test="$T='System.Int32'">int</xsl:when>
					<xsl:when test="$T='System.Int64'">long</xsl:when>
					<xsl:when test="$T='System.SByte'">sbyte</xsl:when>
					<xsl:when test="$T='System.Single'">float</xsl:when>
					<xsl:when test="$T='System.String'">string</xsl:when>
					<xsl:when test="$T='System.UInt16'">ushort</xsl:when>
					<xsl:when test="$T='System.UInt32'">uint</xsl:when>
					<xsl:when test="$T='System.UInt64'">ulong</xsl:when>
					<xsl:when test="$T='System.Void'">void</xsl:when>
	
					<!-- if the type is in the wrt namespace, omit the namespace name -->
					<xsl:when test="not($wrt='') and starts-with($T, concat($wrt,'.')) and not(contains(substring-after($T,concat($wrt,'.')), '.'))">
						<xsl:value-of select="translate (substring-after($T,concat($wrt,'.')), '+', '.')"/>
					</xsl:when>
	
					<!-- if the type is in the System namespace, omit the namespace name -->
					<xsl:when test="starts-with($T, 'System.') and not(contains(substring-after($T, 'System.'), '.'))">
						<xsl:value-of select="translate (substring-after($T,'System.'), '+', '.')"/>
					</xsl:when>
	
					<!-- if the type is in the System.Collections namespace, omit the namespace name -->
					<xsl:when test="starts-with($T, 'System.Collections.') and not(contains(substring-after($T, 'System.Collections.'), '.'))">
						<xsl:value-of select="translate (substring-after($T,'System.Collections.'), '+', '.')"/>
					</xsl:when>

					<xsl:otherwise>
						<xsl:value-of select="translate ($T, '+', '.')"/>
					</xsl:otherwise>
				</xsl:choose>
	</xsl:template>

	<xsl:template name="GetLinkTarget">
		<xsl:param name="type" />
		<xsl:param name="local-suffix" />
		<xsl:param name="remote" />
		<xsl:param name="xmltarget" select='0'/>
		<!-- Search for type in the index.xml file. -->
		<xsl:variable name="typeentry" select="$Index/Overview/Types/Namespace/Type[concat(parent::Namespace/@Name,'.',translate(@Name, '+', '.')) = $type]"/>
		
		<xsl:choose>
			<xsl:when test="count($typeentry)">
				<xsl:value-of select="concat($basepath,$typeentry/parent::Namespace/@Name, '/', $typeentry/@Name)"/>
				<xsl:text>.</xsl:text>
				<xsl:if test="$xmltarget=0"><xsl:value-of select="$ext" /></xsl:if>
				<xsl:if test="$xmltarget=1">xml</xsl:if>
				<xsl:value-of select="$local-suffix" />
			</xsl:when>

			<!-- documentation not available, return empty string -->
			<xsl:when test="$xmltarget = 1">--not-available--</xsl:when>

			<xsl:when test="starts-with($type, 'System.') or 
				starts-with($type, 'Cairo.') or starts-with ($type, 'Commons.Xml.') or
				starts-with($type, 'Mono.GetOptions.') or starts-with($type,'Mono.Math.') or
				starts-with($type, 'Mono.Posix.') or starts-with($type, 'Mono.Remoting.') or
				starts-with($type, 'Mono.Security.') or starts-with($type, 'Mono.Unix.') or
				starts-with($type, 'Mono.Xml.')">
				<xsl:value-of select="$remote" />
			</xsl:when>
			<xsl:otherwise>javascript:alert("Documentation not found.")</xsl:otherwise>
			<!--<xsl:otherwise>javascript:alert("Documentation not found for <xsl:value-of select="$type"/>.")</xsl:otherwise>-->
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="memberlinkprefix">
		<xsl:param name="member" />
		<xsl:choose>
			<xsl:when test="$member/MemberType='Constructor'">C</xsl:when>
			<xsl:when test="$member/MemberType='Method'">M</xsl:when>
			<xsl:when test="$member/MemberType='Property'">P</xsl:when>
			<xsl:when test="$member/MemberType='Field'">F</xsl:when>
			<xsl:when test="$member/MemberType='Event'">E</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="makememberlink">
		<xsl:param name="cref"/>

		<xsl:variable name="fullname">
			<xsl:choose>
				<xsl:when test="starts-with($cref, 'C:') or starts-with($cref, 'T:')">
					<xsl:value-of select="substring($cref, 3)" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="GetTypeName">
						<xsl:with-param name="type" select="substring($cref, 3)"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="memberName">
			<xsl:choose>
				<xsl:when test="starts-with($cref, 'C:') or starts-with($cref, 'T:')"></xsl:when>
				<xsl:otherwise>
					<xsl:text>.</xsl:text>
					<xsl:call-template name="GetMemberName">
						<xsl:with-param name="type" select="substring($cref, 3)" />
						<xsl:with-param name="wrt" select="$fullname"/>
						<xsl:with-param name="isproperty" select="starts-with($cref, 'P:')"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="escaped-type">
			<xsl:call-template name="GetEscapedTypeName">
				<xsl:with-param name="typename" select="$fullname" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:variable name="displayname">
			<xsl:call-template name="GetTypeDisplayName">
				<xsl:with-param name="T" select="$fullname" />
				<xsl:with-param name="wrt" select="$TypeNamespace"/>
			</xsl:call-template>
		</xsl:variable>
		<a>
			<xsl:attribute name="href">
				<xsl:call-template name="GetLinkTarget">
					<xsl:with-param name="type" select="$escaped-type" />
					<xsl:with-param name="local-suffix">#<xsl:value-of select="$cref" /></xsl:with-param>
					<xsl:with-param name="remote">
						<xsl:value-of select="$mono-docs" />
						<xsl:value-of select="$cref" />
					</xsl:with-param>
				</xsl:call-template>
			</xsl:attribute>
			<xsl:value-of select="translate (concat($displayname, $memberName), '+', '.')" />
		</a>
	</xsl:template>

	<xsl:template name="GetTypeName">
		<xsl:param name="type" />
		<xsl:variable name="prefix" select="substring-before($type, '.')" />
		<xsl:variable name="suffix" select="substring-after($type, '.')" />
		<xsl:choose>
			<xsl:when test="contains($type, '(')">
				<xsl:call-template name="GetTypeName">
					<xsl:with-param name="type" select="substring-before($type, '(')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="not(contains($suffix, '.'))">
				<xsl:value-of select="$prefix" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$prefix" />
				<xsl:text>.</xsl:text>
				<xsl:call-template name="GetTypeName">
					<xsl:with-param name="type" select="$suffix" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetMemberName">
		<xsl:param name="type" />
		<xsl:param name="isproperty" select="0"/>
		<xsl:variable name="prefix" select="substring-before($type, '.')" />
		<xsl:variable name="suffix" select="substring-after($type, '.')" />
		<xsl:choose>
			<xsl:when test="contains($type, '(')">
				<xsl:call-template name="GetMemberName">
					<xsl:with-param name="type" select="substring-before($type, '(')" />
				</xsl:call-template>
				<xsl:if test="not($isproperty)">(</xsl:if>
				<xsl:if test="($isproperty)">[</xsl:if>
				<xsl:call-template name="GetMemberArgList">
					<xsl:with-param name="arglist" select="substring-before(substring-after($type, '('), ')')" />
					<xsl:with-param name="wrt" select="$TypeNamespace" />
				</xsl:call-template>
				<xsl:if test="not($isproperty)">)</xsl:if>
				<xsl:if test="($isproperty)">]</xsl:if>
			</xsl:when>
			<xsl:when test="not(contains($suffix, '.'))">
				<xsl:value-of select="$suffix" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="GetMemberName">
					<xsl:with-param name="type" select="$suffix" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetMemberArgList">
		<xsl:param name="arglist" />
		<xsl:param name="wrt" select="''"/>
		<xsl:choose>
			<xsl:when test="contains($arglist, ',')">
				<xsl:call-template name="GetMemberArgList">
					<xsl:with-param name="arglist" select="substring-before($arglist, ',')" />
					<xsl:with-param name="wrt" select="$wrt" />
				</xsl:call-template>
				<xsl:text>, </xsl:text>
				<xsl:call-template name="GetMemberArgList">
					<xsl:with-param name="arglist" select="substring-after($arglist, ',')" />
					<xsl:with-param name="wrt" select="$wrt" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="GetTypeDisplayName">
					<xsl:with-param name="T" select="$arglist"/>
					<xsl:with-param name="wrt" select="$wrt"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- Transforms the contents of the selected node containing a cref into a hyperlink. -->
	<xsl:template match="*|@*" mode="cref">
		<xsl:call-template name="makememberlink">
			<xsl:with-param name="cref" select="."/>
		</xsl:call-template>
		<!--
		<a>
			<xsl:attribute name="href"><xsl:value-of select="."/></xsl:attribute>
			<xsl:value-of select="substring-after(., ':')"/></a>
			-->
	</xsl:template>

	<xsl:template name="membertypeplural">
		<xsl:param name="name"/>
		<xsl:choose>
		<xsl:when test="$name='Constructor'">Constructors</xsl:when>
		<xsl:when test="$name='Property'">Properties</xsl:when>
		<xsl:when test="$name='Method'">Methods</xsl:when>
		<xsl:when test="$name='Field'">Fields</xsl:when>
		<xsl:when test="$name='Event'">Events</xsl:when>
		<xsl:when test="$name='Operator'">Operators</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="membertypeplurallc">
		<xsl:param name="name"/>
		<xsl:choose>
		<xsl:when test="$name='Constructor'">constructors</xsl:when>
		<xsl:when test="$name='Property'">properties</xsl:when>
		<xsl:when test="$name='Method'">methods</xsl:when>
		<xsl:when test="$name='Field'">fields</xsl:when>
		<xsl:when test="$name='Event'">events</xsl:when>
		<xsl:when test="$name='Operator'">operators</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="gettypetype">
		<xsl:variable name="sig" select="concat(' ', TypeSignature[@Language='C#']/@Value, ' ')"/>
		<xsl:choose>
		<xsl:when test="contains($sig,'class')">Class</xsl:when>
		<xsl:when test="contains($sig,'enum')">Enumeration</xsl:when>
		<xsl:when test="contains($sig,'struct')">Structure</xsl:when>
		<xsl:when test="contains($sig,'delegate')">Delegate</xsl:when>
		</xsl:choose>
	</xsl:template>

	<!-- Ensures that the resuting node is not surrounded by a para tag. -->
	<xsl:template match="*" mode="notoppara">
		<xsl:choose>
		<xsl:when test="count(*) = 1 and count(para)=1">
			<xsl:apply-templates select="para/node()"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates select="."/>
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="para">
		<p>
			<xsl:apply-templates/>
		</p>
	</xsl:template>

	<xsl:template match="paramref">
		<i><xsl:value-of select="@name"/>
				<xsl:apply-templates/>
		</i>
	</xsl:template>

	<xsl:template match="block[@type='note']">
		<p>
		<i>Note: </i>
				<xsl:apply-templates/>
		</p>
	</xsl:template>
	<xsl:template match="block[@type='behaviors']">
		<div class="Subsection">Operation</div><p><xsl:apply-templates/></p>
	</xsl:template>
	<xsl:template match="block[@type='overrides']">
		<div class="Subsection">Note to Inheritors</div><p><xsl:apply-templates/></p>
	</xsl:template>
	<xsl:template match="block[@type='usage']">
		<div class="Subsection">Usage</div><p><xsl:apply-templates/></p>
	</xsl:template>


	<xsl:template match="c">
		<tt><font size="-1">
			<xsl:apply-templates/>	
		</font></tt>
	</xsl:template>
	<xsl:template match="c//para">
		<xsl:apply-templates/><br/>	
	</xsl:template>
	
	<xsl:template match="code">
		<table class="CodeExampleTable">
		<tr><td><b><font size="-1"><xsl:value-of select="@lang"/> Example</font></b></td></tr>
		<tr><td>
			<!--<xsl:value-of select="monodoc:Colorize(string(descendant-or-self::text()), string(@lang))" disable-output-escaping="yes" />-->
			<pre>
				<xsl:value-of select="." />
			</pre>
		</td></tr>
		</table>
	</xsl:template>

	<xsl:template match="onequarter">
		1/4
	</xsl:template>
	<xsl:template match="pi">pi</xsl:template>
	<xsl:template match="theta">theta</xsl:template>
	<xsl:template match="subscript">
		<sub><xsl:value-of select="@term"/></sub>
	</xsl:template>
	<xsl:template match="superscript">
		<sup><xsl:value-of select="@term"/></sup>
	</xsl:template>

	<!-- tabular data
		example:

			<list type="table">
				<listheader>
					<term>First Col Header</term>
					<description>Second Col Header</description>
					<description>Third Col Header</description>
				</listheader>
				<item>
					<term>First Row First Col</term>
					<description>First Row Second Col</description>
					<description>First Row Third Col</description>
				</item>
				<item>
					<term>Second Row First Col</term>
					<description>Second Row Second Col</description>
					<description>Second Row Third Col</description>
				</item>
			</list>
	-->

	<xsl:template match="list[@type='table']">
		<table border="1" cellpadding="3">
		<tr valign="top">
			<th><xsl:apply-templates select="listheader/term" mode="notoppara"/></th>
			<xsl:for-each select="listheader/description">
				<th><xsl:apply-templates mode="notoppara"/></th>
			</xsl:for-each>
		</tr>

		<xsl:for-each select="item">
			<tr valign="top">
			<td>
				<xsl:apply-templates select="term" mode="notoppara"/>
			</td>
			<xsl:for-each select="description">
				<td>
					<xsl:apply-templates mode="notoppara"/>
				</td>
			</xsl:for-each>
			</tr>
		</xsl:for-each>

		</table>
	</xsl:template>

	<xsl:template match="list[@type='bullet']">
		<ul>
			<xsl:for-each select="item">
				<li>
					<xsl:apply-templates select="term" mode="notoppara"/>
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>
	<xsl:template match="list[@type='number']">
		<ol>
			<xsl:for-each select="item">
				<li>
					<xsl:apply-templates select="term" mode="notoppara"/>
				</li>
			</xsl:for-each>
		</ol>
	</xsl:template>

	<xsl:template match="list">
		[<i>The '<xsl:value-of select="@type"/>' type of list has not been implemented in the ECMA stylesheet.</i>]
		
		<xsl:message>
		[<i>The '<xsl:value-of select="@type"/>' type of list has not been implemented in the ECMA stylesheet.</i>]
		</xsl:message>
	</xsl:template>

	<xsl:template match="see[@cref]">
		<xsl:choose>
		<xsl:when test="not(substring-after(@cref, 'T:')='')">
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="@cref"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="makememberlink">
				<xsl:with-param name="cref" select="@cref"/>
			</xsl:call-template>
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="see[@langword]">
		<!--<a href="{@langword}"><xsl:value-of select="@langword"/></a>-->
		<tt><xsl:value-of select="@langword"/></tt>
	</xsl:template>
	
	<xsl:template name="GetInheritedMembers">
		<xsl:param name="declaringtype"/>
		<xsl:param name="generictypereplacements"/>
		<xsl:param name="listmembertype"/>
		<xsl:param name="showprotected"/>
		<xsl:param name="showstatic" select='1'/>

		<Members Name="{$declaringtype/@Name}" FullName="{$declaringtype/@FullName}">
		
		<xsl:copy-of select="$generictypereplacements"/>

		<!-- Get all members in this type that are of listmembertype and are either
			protected or not protected according to showprotected. -->
		<xsl:copy-of select="$declaringtype/Members/Member
			[(MemberType=$listmembertype or ($listmembertype='Operator' and MemberType='Method'))]
			[$showprotected=contains(concat(' ',MemberSignature[@Language='C#']/@Value),' protected ')]
			[($listmembertype='Method' and not(starts-with(@MemberName,'op_')))
				or ($listmembertype='Operator' and starts-with(@MemberName,'op_'))
				or (not($listmembertype='Method') and not($listmembertype='Operator'))]
			[$showstatic or not(contains(MemberSignature[@Language='C#']/@Value,' static '))]
			"/>

		<Docs>
			<xsl:copy-of select="$declaringtype/Docs/typeparam" />
		</Docs>
			
		</Members>

		<xsl:if test="not($listmembertype='Constructor') and count($declaringtype/Base/BaseTypeName)=1">
			<xsl:variable name="basedocsfile">
				<xsl:call-template name="GetLinkTarget">
					<xsl:with-param name="type">
						<xsl:call-template name="GetEscapedTypeName">
							<xsl:with-param name="typename" select="$declaringtype/Base/BaseTypeName" />
						</xsl:call-template>
					</xsl:with-param>
					<xsl:with-param name="local-suffix" />
					<xsl:with-param name="remote"/>
					<xsl:with-param name="xmltarget" select='1'/>
				</xsl:call-template>
			</xsl:variable>

			<xsl:if test="not($basedocsfile='--not-available--')">
				<xsl:call-template name="GetInheritedMembers">
					<xsl:with-param name="listmembertype" select="$listmembertype"/>
					<xsl:with-param name="showprotected" select="$showprotected"/>
					<xsl:with-param name="declaringtype" select="document($basedocsfile,.)/Type"/>
					<xsl:with-param name="generictypereplacements" select="$declaringtype/Base/BaseTypeArguments/*"/>
					<xsl:with-param name="showstatic" select='0'/>
				</xsl:call-template>
			</xsl:if>
		</xsl:if>
	</xsl:template>
	
	<!-- Lists the members in the current Type node.
		 Only lists members of type listmembertype.
		 Displays the signature in siglanguage.
		 showprotected = true() or false()
	-->
	<xsl:template name="ListMembers">
		<xsl:param name="listmembertype"/>
		<xsl:param name="showprotected"/>

		<!-- get name and namespace of current type -->
		<xsl:variable name="TypeFullName" select="@FullName"/>
		<xsl:variable name="TypeName" select="@Name"/>		
		<xsl:variable name="TypeNamespace" select="substring-before(@FullName, concat('.',@Name))"/>
		
		<xsl:variable name="MEMBERS">
			<xsl:call-template name="GetInheritedMembers">
				<xsl:with-param name="listmembertype" select="$listmembertype"/>
				<xsl:with-param name="showprotected" select="$showprotected"/>
				<xsl:with-param name="declaringtype" select="."/>
			</xsl:call-template>
		</xsl:variable>
		
		<!--
		<xsl:variable name="MEMBERS" select="
			$ALLMEMBERS/Member
			[(MemberType=$listmembertype or ($listmembertype='Operator' and MemberType='Method'))]
			[$showprotected=contains(MemberSignature[@Language='C#']/@Value,'protected')]
			[($listmembertype='Method' and not(starts-with(@MemberName,'op_')))
				or ($listmembertype='Operator' and starts-with(@MemberName,'op_'))
				or (not($listmembertype='Method') and not($listmembertype='Operator'))]
			"/>
		-->
		
		<!-- if there aren't any, skip this -->
		<xsl:if test="count($MEMBERS/Member)">

		<!-- header -->
		<h3>
			<xsl:if test="$showprotected">Protected </xsl:if>
			<xsl:call-template name="membertypeplural"><xsl:with-param name="name" select="$listmembertype"/></xsl:call-template>
			</h3>

		<div class="SubsectionBox">
		<table class="MembersListing">

		<xsl:for-each select="$MEMBERS/Member">
			<!--<xsl:sort select="contains(MemberSignature[@Language='C#']/@Value,' static ')" data-type="text"/>-->
			<xsl:sort select="@MemberName = 'op_Implicit' or @MemberName = 'op_Explicit'"/>
			<xsl:sort select="@MemberName" data-type="text"/>
			<xsl:sort select="count(Parameters/Parameter)"/>
			<xsl:sort select="Parameters/Parameter/@Type"/>
			
			<xsl:variable name="linkid">
				<xsl:if test="not(parent::Members/@FullName = $TypeFullName)">
					<xsl:call-template name="GetLinkTarget">
						<xsl:with-param name="type">
							<xsl:call-template name="GetEscapedTypeName">
								<xsl:with-param name="typename" select="parent::Members/@FullName" />
							</xsl:call-template>
						</xsl:with-param>
						<xsl:with-param name="local-suffix"/>
						<xsl:with-param name="remote"/>
					</xsl:call-template>
				</xsl:if>

				<xsl:text>#</xsl:text>
				<xsl:call-template name="GetLinkId" >
					<xsl:with-param name="type" select="parent::Members" />
					<xsl:with-param name="member" select="." />
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:variable name="isinherited">
				<xsl:if test="not(parent::Members/@FullName = $TypeFullName)">
					<xsl:text> (</xsl:text>
					<i>
					<xsl:text>Inherited from </xsl:text>
					<xsl:call-template name="maketypelink">
						<xsl:with-param name="type" select="parent::Members/@FullName"/>
						<xsl:with-param name="wrt" select="$TypeNamespace"/>
					</xsl:call-template>
					<xsl:text>.</xsl:text>
					</i>
					<xsl:text>)</xsl:text>
				</xsl:if>
			</xsl:variable>

			<tr valign="top">

			<xsl:choose>
				<!-- constructor listing -->
				<xsl:when test="MemberType='Constructor'">
					<!-- link to constructor page -->
					<td>
					<div>
					<b>
					<a href="{$linkid}">
						<xsl:call-template name="GetConstructorName">
							<xsl:with-param name="type" select="parent::Members" />
							<xsl:with-param name="ctor" select="." />
						</xsl:call-template>
					</a>
					</b>

					<!-- argument list -->
					<xsl:value-of select="'('"/>
						<xsl:for-each select="Parameters/Parameter">
							<xsl:if test="not(position()=1)">, </xsl:if>
							
							<xsl:call-template name="ShowParameter">
								<xsl:with-param name="Param" select="."/>
								<xsl:with-param name="TypeNamespace" select="$TypeNamespace"/>
								<xsl:with-param name="prototype" select="true()"/>
							</xsl:call-template>
						</xsl:for-each>
					<xsl:value-of select="')'"/>
					</div>

					<!-- description -->
					<xsl:apply-templates select="Docs/summary" mode="notoppara"/>

					</td>
				</xsl:when>

				<!-- field, property and event listing -->
				<xsl:when test="MemberType='Field' or MemberType='Property' or MemberType='Event'">
					<td>

					<!-- link to member page -->
					<b>
					<a href="{$linkid}">
						<xsl:value-of select="@MemberName"/>
					</a>
					</b>

					<!-- argument list for accessors -->
					<xsl:if test="Parameters/Parameter">
					<xsl:value-of select="'['"/>
						<xsl:for-each select="Parameters/Parameter">
							<xsl:if test="not(position()=1)">, </xsl:if>
							
							<xsl:call-template name="ShowParameter">
								<xsl:with-param name="Param" select="."/>
								<xsl:with-param name="TypeNamespace" select="$TypeNamespace"/>
								<xsl:with-param name="prototype" select="true()"/>
							</xsl:call-template>

						</xsl:for-each>
					<xsl:value-of select="']'"/>
					</xsl:if>

					<!-- check if it has get and set accessors -->
					<xsl:if test="MemberType='Property' and not(contains(MemberSignature[@Language='C#']/@Value, 'set;'))">
					[read-only]
					</xsl:if>
					<xsl:if test="MemberType='Property' and not(contains(MemberSignature[@Language='C#']/@Value, 'get;'))">
					[write-only]
					</xsl:if>

					<xsl:if test="contains(MemberSignature[@Language='C#']/@Value,'this[')">
						<div><i>default property</i></div>
					</xsl:if>

					<div>
					<xsl:call-template name="getmodifiers">
						<xsl:with-param name="sig" select="MemberSignature[@Language='C#']/@Value"/>
						<xsl:with-param name="protection" select="false()"/>
						<xsl:with-param name="inheritance" select="true()"/>
						<xsl:with-param name="extra" select="false()"/>
					</xsl:call-template>
					</div>

					</td>
					<td>
					<!-- field/property type -->
					<xsl:if test="not(MemberType='Event')">
						<i><xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates></i>
						<xsl:if test="MemberValue"> (<xsl:value-of select="MemberValue"/>)</xsl:if>
						<xsl:text>. </xsl:text>
					</xsl:if>

					<!-- description -->
					<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
					
					<xsl:copy-of select="$isinherited"/>
					</td>
				</xsl:when>

				<!-- method listing -->
				<xsl:when test="$listmembertype='Method'">
					<td>
						<xsl:call-template name="getmodifiers">
							<xsl:with-param name="sig" select="MemberSignature[@Language='C#']/@Value"/>
							<xsl:with-param name="protection" select="false()"/>
							<xsl:with-param name="inheritance" select="true()"/>
							<xsl:with-param name="extra" select="false()"/>
						</xsl:call-template>
					</td>

					<td>

					<!-- link to method page -->
					<b>
					<a href="{$linkid}">
						<xsl:value-of select="@MemberName"/>
					</a>
					</b>

					<!-- argument list -->
					<xsl:value-of select="'('"/>
						<xsl:for-each select="Parameters/Parameter">
							<xsl:if test="not(position()=1)">, </xsl:if>
							
							<xsl:call-template name="ShowParameter">
								<xsl:with-param name="Param" select="."/>
								<xsl:with-param name="TypeNamespace" select="$TypeNamespace"/>
								<xsl:with-param name="prototype" select="true()"/>
							</xsl:call-template>

						</xsl:for-each>
					<xsl:value-of select="')'"/>

					<!-- return type -->
					<xsl:if test="not(ReturnValue/ReturnType='System.Void')">
						<nobr>
						<xsl:text> : </xsl:text>
						<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
						</nobr>
					</xsl:if>
					
					<!-- description -->
					<div>
						<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
						<xsl:copy-of select="$isinherited"/>
					</div>
					</td>
				</xsl:when>

				<xsl:when test="$listmembertype='Operator'">
					<td>

					<!-- link to operator page -->
					<xsl:choose>
					<xsl:when test="@MemberName='op_Implicit' or @MemberName='op_Explicit'">
						<b>
						<a href="{$linkid}">
							<xsl:text>Conversion</xsl:text>
							<xsl:choose>
							<xsl:when test="ReturnValue/ReturnType = //Type/@FullName">
								<xsl:text> From </xsl:text>
								<xsl:value-of select="Parameters/Parameter/@Type"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text> to </xsl:text>
								<xsl:value-of select="ReturnValue/ReturnType"/>
							</xsl:otherwise>
							</xsl:choose>
						</a>
						</b>						

						<xsl:choose>
						<xsl:when test="@MemberName='op_Implicit'">
							<xsl:text>(Implicit)</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>(Explicit)</xsl:text>
						</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="count(Parameters/Parameter)=1">
						<b>
						<a href="{$linkid}">
							<xsl:value-of select="substring-after(@MemberName, 'op_')"/>
						</a>
						</b>
					</xsl:when>
					<xsl:otherwise>
						<b>
						<a href="{$linkid}">
							<xsl:value-of select="substring-after(@MemberName, 'op_')"/>
						</a>
						</b>
						<xsl:value-of select="'('"/>
							<xsl:for-each select="Parameters/Parameter">
								<xsl:if test="not(position()=1)">, </xsl:if>
								
								<xsl:call-template name="ShowParameter">
									<xsl:with-param name="Param" select="."/>
									<xsl:with-param name="TypeNamespace" select="$TypeNamespace"/>
									<xsl:with-param name="prototype" select="true()"/>
								</xsl:call-template>
	
							</xsl:for-each>
						<xsl:value-of select="')'"/>
					</xsl:otherwise>
					</xsl:choose>
			

					<!-- description -->
					<div>
						<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
					</div>
					</td>
				</xsl:when>
				
				<xsl:otherwise>
					<!-- Other types: just provide a link -->
					<a href="{$linkid}">
						<xsl:value-of select="@MemberName"/>
					</a>
				</xsl:otherwise>
			</xsl:choose>
			
			<!--<tt><xsl:value-of select="MemberSignature[@Language='C#']/@Value"/></tt><br/>-->

			</tr>
		</xsl:for-each>

		</table>
		</div>

		</xsl:if>

	</xsl:template>

	<xsl:template name="GetLinkName">
		<xsl:param name="type"/>
		<xsl:param name="member"/>
		<xsl:call-template name="memberlinkprefix">
			<xsl:with-param name="member" select="$member"/>
		</xsl:call-template>
		<xsl:text>:</xsl:text>
		<xsl:call-template name="GetEscapedTypeName">
			<xsl:with-param name="typename" select="$type/@FullName" />
		</xsl:call-template>
		<xsl:if test="$member/MemberType != 'Constructor'">
			<xsl:text>.</xsl:text>
			<xsl:call-template name="GetGenericName">
				<xsl:with-param name="membername" select="$member/@MemberName" />
				<xsl:with-param name="member" select="$member" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<xsl:template name="GetGenericName">
		<xsl:param name="membername" />
		<xsl:param name="member" />
		<xsl:variable name="numgenargs" select="count($member/Docs/typeparam)" />
		<xsl:choose>
			<xsl:when test="$numgenargs = 0">
				<xsl:value-of select="$membername" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="contains($membername, '&lt;')">
					<xsl:value-of select="substring-before ($membername, '&lt;')" />
				</xsl:if>
				<xsl:text>``</xsl:text>
				<xsl:value-of select="$numgenargs" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetEscapedTypeName">
		<xsl:param name="typename" />
		<xsl:variable name="base" select="substring-before ($typename, '&lt;')" />

		<xsl:choose>
			<xsl:when test="$base != ''">
				<xsl:value-of select="translate ($base, '+', '.')" />
				<xsl:text>`</xsl:text>
				<xsl:call-template name="GetGenericArgumentCount">
					<xsl:with-param name="arglist" select="substring-after ($typename, '&lt;')" />
					<xsl:with-param name="count">1</xsl:with-param>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise><xsl:value-of select="translate ($typename, '+', '.')" /></xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetGenericArgumentCount">
		<xsl:param name="arglist" />
		<xsl:param name="count" />

		<xsl:variable name="rest">
			<xsl:call-template name="SkipTypeArgument">
				<xsl:with-param name="s" select="$arglist" />
			</xsl:call-template>
		</xsl:variable>

		<xsl:choose>
			<xsl:when test="$arglist = '' or $rest = ''">
				<xsl:message terminate="yes">
!WTF? arglist=<xsl:value-of select="$arglist" />; rest=<xsl:value-of select="$rest" />
				</xsl:message>
			</xsl:when>
			<xsl:when test="starts-with ($rest, '>')">
				<xsl:value-of select="$count" />
				<xsl:call-template name="GetEscapedTypeName">
					<xsl:with-param name="typename" select="substring-after ($rest, '>')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="starts-with ($rest, ',')">
				<xsl:call-template name="GetGenericArgumentCount">
					<xsl:with-param name="arglist" select="substring-after ($rest, ',')" />
					<xsl:with-param name="count" select="$count+1" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:message terminate="yes">
!WTF 2? arglist=<xsl:value-of select="$arglist" />; rest=<xsl:value-of select="$rest" />
				</xsl:message>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="SkipTypeArgument">
		<xsl:param name="s" />

		<xsl:variable name="c"  select="substring-before ($s, ',')" />
		<xsl:variable name="lt" select="substring-before ($s, '&lt;')" />
		<xsl:variable name="gt" select="substring-before ($s, '&gt;')" />

		<xsl:choose>
			<!--
			Have to select between two `s' patterns:
			A,B>: need to return ",B>"
			Foo<A,B>>: Need to forward to SkipGenericArgument to eventually return ">"
			-->
			<xsl:when test="($c != '' and $lt != '' and string-length ($c) &lt; string-length ($lt)) or
				($c != '' and $lt = '')">
				<xsl:text>,</xsl:text>
				<xsl:value-of select="substring-after ($s, ',')" />
			</xsl:when>
			<xsl:when test="$lt != '' and ($gt != '' and string-length ($lt) &lt; string-length ($gt))">
				<xsl:call-template name="SkipGenericArgument">
					<xsl:with-param name="s" select="$s" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$gt != ''">
				<xsl:text>&gt;</xsl:text>
				<xsl:value-of select="substring-after ($s, '&gt;')" />
			</xsl:when>
			<xsl:otherwise><xsl:value-of select="$s" /></xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!--
	when given 'Foo<A,Bar<Baz<C,D,E>>>>', returns '>'
	when given 'Foo<A,Bar<Baz<C,D,E>>>,', returns ','
	(basically, it matches '<' to '>' and "skips" the intermediate contents.
	  -->
	<xsl:template name="SkipGenericArgument">
		<xsl:param name="s" />

		<xsl:variable name="c"  select="substring-before ($s, ',')" />
		<xsl:variable name="lt" select="substring-before ($s, '&lt;')" />
		<xsl:variable name="gt" select="substring-before ($s, '&gt;')" />

		<xsl:choose>
			<xsl:when test="$c != '' and string-length ($c) &lt; string-length ($lt)">
				<!-- within 'C,D,E>', so consume rest of template -->
				<xsl:call-template name="SkipGenericArgument">
					<xsl:with-param name="s" select="substring-after ($s, ',')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$lt != '' and string-length ($lt) &lt; string-length ($gt)">
				<!-- within 'Foo<A...'; look for matching '>' -->
				<xsl:call-template name="SkipGenericArgument">
					<xsl:with-param name="s" select="substring-after ($s, '&lt;')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$gt != ''">
				<xsl:value-of select="substring-after ($s, '&gt;')" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$s" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetEscapedParameter">
		<xsl:param name="orig-parameter-type" />
		<xsl:param name="parameter-type" />
		<xsl:param name="parameter-types" />
		<xsl:param name="escape" />
		<xsl:param name="index" />

		<xsl:choose>
			<xsl:when test="$index &gt; count($parameter-types)">
				<xsl:if test="$parameter-type != $orig-parameter-type">
					<xsl:value-of select="$parameter-type" />
				</xsl:if>
				<!-- ignore -->
			</xsl:when>
			<xsl:when test="$parameter-types[position() = $index]/@name = $parameter-type">
				<xsl:value-of select="concat ($escape, $index - 1)" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="typeparam" select="$parameter-types[position() = $index]/@name" />
				<xsl:call-template name="GetEscapedParameter">
					<xsl:with-param name="orig-parameter-type" select="$orig-parameter-type" />
					<xsl:with-param name="parameter-type">
						<xsl:call-template name="Replace">
							<xsl:with-param name="s">
								<xsl:call-template name="Replace">
									<xsl:with-param name="s">
										<xsl:call-template name="Replace">
											<xsl:with-param name="s">
												<xsl:call-template name="Replace">
													<xsl:with-param name="s" select="$parameter-type"/>
													<xsl:with-param name="from" select="concat('&lt;', $typeparam, '&gt;')" />
													<xsl:with-param name="to" select="concat('&lt;', $escape, $index - 1, '&gt;')" />
												</xsl:call-template>
											</xsl:with-param>
											<xsl:with-param name="from" select="concat('&lt;', $typeparam, ',')" />
											<xsl:with-param name="to" select="concat('&lt;', $escape, $index - 1, ',')" />
										</xsl:call-template>
									</xsl:with-param>
									<xsl:with-param name="from" select="concat (',', $typeparam, '&gt;')" />
									<xsl:with-param name="to" select="concat(',', $escape, $index - 1, '&gt;')" />
								</xsl:call-template>
							</xsl:with-param>
							<xsl:with-param name="from" select="concat (',', $typeparam, ',')" />
							<xsl:with-param name="to" select="concat(',', $escape, $index - 1, ',')" />
						</xsl:call-template>
					</xsl:with-param>
					<xsl:with-param name="parameter-types" select="$parameter-types" />
					<xsl:with-param name="typeparam" select="$typeparam" />
					<xsl:with-param name="escape" select="$escape" />
					<xsl:with-param name="index" select="$index + 1" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="GetLinkId">
		<xsl:param name="type"/>
		<xsl:param name="member"/>
		<xsl:call-template name="GetLinkName">
			<xsl:with-param name="type" select="$type" />
			<xsl:with-param name="member" select="$member" />
		</xsl:call-template>
		<xsl:if test="count($member/Parameters/Parameter) &gt; 0">
			<xsl:text>(</xsl:text>
			<xsl:for-each select="Parameters/Parameter">
				<xsl:if test="not(position()=1)">,</xsl:if>
				<xsl:call-template name="GetParameterType">
					<xsl:with-param name="type" select="$type" />
					<xsl:with-param name="member" select="$member" />
					<xsl:with-param name="parameter" select="." />
				</xsl:call-template>
			</xsl:for-each>
			<xsl:text>)</xsl:text>
		</xsl:if>
		<xsl:if test="$member/@MemberName='op_Implicit' or $member/@MemberName='op_Explicit'">
			<xsl:text>~</xsl:text>
			<xsl:call-template name="GetParameterType">
				<xsl:with-param name="type" select="$type" />
				<xsl:with-param name="member" select="$member" />
				<xsl:with-param name="parameter">
					<Parameter Type="{$member/ReturnValue/ReturnType}" />
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<!-- 
	  - what should be <xsl:value-of select="@Type" /> becomes a nightmare once
		- generics enter the picture, since a parameter type could come from the
		- type itelf (becoming `N) or from the method (becoming ``N).
	  -->
	<xsl:template name="GetParameterType">
		<xsl:param name="type" />
		<xsl:param name="member" />
		<xsl:param name="parameter" />

		<!-- the actual parameter type -->
		<xsl:variable name="ptype">
			<xsl:choose>
				<xsl:when test="contains($parameter/@Type, '[')">
					<xsl:value-of select="substring-before ($parameter/@Type, '[')" />
				</xsl:when>
				<xsl:when test="contains($parameter/@Type, '&amp;')">
					<xsl:value-of select="substring-before ($parameter/@Type, '&amp;')" />
				</xsl:when>
				<xsl:when test="contains($parameter/@Type, '*')">
					<xsl:value-of select="substring-before ($parameter/@Type, '*')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$parameter/@Type" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- parameter modifiers -->
		<xsl:variable name="pmodifier">
			<xsl:call-template name="Replace">
				<xsl:with-param name="s" select="substring-after ($parameter/@Type, $ptype)" />
				<xsl:with-param name="from">&amp;</xsl:with-param>
				<xsl:with-param name="to">@</xsl:with-param>
			</xsl:call-template>
		</xsl:variable>

		<xsl:variable name="gen-type">
			<xsl:call-template name="GetEscapedParameter">
				<xsl:with-param name="orig-parameter-type" select="$ptype" />
				<xsl:with-param name="parameter-type">
					<xsl:variable name="nested">
						<xsl:call-template name="GetEscapedParameter">
							<xsl:with-param name="orig-parameter-type" select="$ptype" />
							<xsl:with-param name="parameter-type" select="$ptype" />
							<xsl:with-param name="parameter-types" select="$type/Docs/typeparam" />
							<xsl:with-param name="escape" select="'`'" />
							<xsl:with-param name="index" select="1" />
						</xsl:call-template>
					</xsl:variable>
					<xsl:choose>
						<xsl:when test="$nested != ''">
							<xsl:value-of select="$nested" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$ptype" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:with-param>
				<xsl:with-param name="parameter-types" select="$member/Docs/typeparam" />
				<xsl:with-param name="escape" select="'``'" />
				<xsl:with-param name="index" select="1" />
			</xsl:call-template>
		</xsl:variable>

		<!-- the actual parameter type -->
		<xsl:variable name="parameter-type">
			<xsl:choose>
				<xsl:when test="$gen-type != ''">
					<xsl:value-of select="$gen-type" />
					<xsl:value-of select="$pmodifier" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="concat($ptype, $pmodifier)" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- s/</{/g; s/>/}/g; so that less escaping is needed. -->
		<xsl:call-template name="Replace">
			<xsl:with-param name="s">
				<xsl:call-template name="Replace">
					<xsl:with-param name="s" select="translate ($parameter-type, '+', '.')" />
					<xsl:with-param name="from">&gt;</xsl:with-param>
					<xsl:with-param name="to">}</xsl:with-param>
				</xsl:call-template>
			</xsl:with-param>
			<xsl:with-param name="from">&lt;</xsl:with-param>
			<xsl:with-param name="to">{</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="Replace">
		<xsl:param name="s" />
		<xsl:param name="from" />
		<xsl:param name="to" />
		<xsl:choose>
			<xsl:when test="not(contains($s, $from))">
				<xsl:value-of select="$s" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="prefix" select="substring-before($s, $from)"/>
				<xsl:variable name="suffix" select="substring-after($s, $from)" />
				<xsl:value-of select="$prefix" />
				<xsl:value-of select="$to" />
				<xsl:call-template name="Replace">
					<xsl:with-param name="s" select="$suffix" />
					<xsl:with-param name="from" select="$from" />
					<xsl:with-param name="to" select="$to" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="getmodifiers">
		<xsl:param name="sig"/>
		<xsl:param name="protection" select="true()"/>
		<xsl:param name="inheritance" select="true()"/>
		<xsl:param name="extra" select="true()"/>
		<xsl:param name="typetype" select="false()"/>

		<xsl:variable name="Sig" select="concat(' ', $sig, ' ')"/>

		<xsl:if test="$protection">
			<xsl:if test="contains($Sig, ' public ')">public </xsl:if>
			<xsl:if test="contains($Sig, ' private ')">private </xsl:if>
			<xsl:if test="contains($Sig, ' protected ')">protected </xsl:if>
			<xsl:if test="contains($Sig, ' internal ')">internal </xsl:if>
		</xsl:if>

		<xsl:if test="contains($Sig, ' static ')">static </xsl:if>
		<xsl:if test="contains($Sig, ' abstract ')">abstract </xsl:if>
		<xsl:if test="contains($Sig, ' operator ')">operator </xsl:if>

		<xsl:if test="contains($Sig, ' const ')">const </xsl:if>
		<xsl:if test="contains($Sig, ' readonly ')">readonly </xsl:if>

		<xsl:if test="$inheritance">
			<xsl:if test="contains($Sig, ' override ')">override </xsl:if>
			<xsl:if test="contains($Sig, ' new ')">override </xsl:if>
		</xsl:if>

		<xsl:if test="$extra">
			<xsl:if test="contains($Sig, ' sealed ')">sealed </xsl:if>
			<xsl:if test="contains($Sig, ' virtual ')">virtual </xsl:if>

			<xsl:if test="contains($Sig, ' extern ')">extern </xsl:if>
			<xsl:if test="contains($Sig, ' checked ')">checked </xsl:if>
			<xsl:if test="contains($Sig, ' unsafe ')">unsafe </xsl:if>
			<xsl:if test="contains($Sig, ' volatile ')">volatile </xsl:if>
			<xsl:if test="contains($Sig, ' explicit ')">explicit </xsl:if>
			<xsl:if test="contains($Sig, ' implicit ')">implicit </xsl:if>
		</xsl:if>

		<xsl:if test="$typetype">
			<xsl:if test="contains($Sig, ' class ')">class </xsl:if>
			<xsl:if test="contains($Sig, ' interface ')">interface </xsl:if>
			<xsl:if test="contains($Sig, ' struct ')">struct </xsl:if>
			<xsl:if test="contains($Sig, ' delegate ')">delegate </xsl:if>
			<xsl:if test="contains($Sig, ' enum ')">enum </xsl:if>
		</xsl:if>
	</xsl:template>

	<xsl:template name="GetTypeDescription">
		<xsl:variable name="sig" select="TypeSignature[@Language='C#']/@Value"/>
		<xsl:choose>
			<xsl:when test="contains($sig, ' class ')">Class</xsl:when>
			<xsl:when test="contains($sig, ' interface ')">Interface</xsl:when>
			<xsl:when test="contains($sig, ' struct ')">Struct</xsl:when>
			<xsl:when test="contains($sig, ' delegate ')">Delegate</xsl:when>
			<xsl:when test="contains($sig, ' enum ')">Enum</xsl:when>
		</xsl:choose>
	</xsl:template>
	

</xsl:stylesheet>
