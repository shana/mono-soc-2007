<?xml version="1.0"?>

<!--
	mono-ecma.xsl: ECMA-style docs to HTML stylesheet trasformation

	Author: Joshua Tauberer (tauberer@for.net)

	TODO:
		split this into multiple files
-->

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:monodoc="monodoc:///extensions"
	exclude-result-prefixes="monodoc"
	>
	
	<xsl:output omit-xml-declaration="yes" />

	<!-- TEMPLATE PARAMETERS -->

	<xsl:param name="language" select="'C#'"/>
	<xsl:param name="show"/>
	<xsl:param name="membertype"/>
	<xsl:param name="namespace"/>
	<xsl:param name="index"/>

	<!-- THE MAIN RENDERING TEMPLATE -->

	<xsl:template match="Type|elements">
		<!-- The namespace that the current type belongs to. -->
		<xsl:variable name="TypeNamespace" select="substring(@FullName, 1, string-length(@FullName) - string-length(@Name) - 1)"/>

		<!-- HEADER -->

		<table class="HeaderTable" width="100%" cellpadding="5">
			<tr bgcolor="#b0c4de"><td>
			<i>Mono Class Library</i>

				<xsl:choose>
					<xsl:when test="$show='masteroverview'">
					</xsl:when>
					<xsl:when test="$show='typeoverview'">
						:
						<a>
							<xsl:attribute name="href">N:<xsl:value-of select="$TypeNamespace"/></xsl:attribute>
							<xsl:value-of select="$TypeNamespace"/> Namespace</a>						
					</xsl:when>
					<xsl:when test="$show='members'">
						:
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/></xsl:attribute>
							<xsl:value-of select="@FullName"/> Overview</a>
					</xsl:when>
					<xsl:when test="$show='member' or $show='overloads'">
						:
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/></xsl:attribute>
							<xsl:value-of select="@FullName"/> Overview</a>
						|
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/*</xsl:attribute>
							Members</a>		
					</xsl:when>
					<xsl:when test="$show='namespace'">
						:
						<a href="root:/classlib">Namespaces</a>
					</xsl:when>
				</xsl:choose>
			
			<h3>
				<xsl:choose>
					<xsl:when test="$show='masteroverview'">
						Master Overview
					</xsl:when>
					<xsl:when test="$show='typeoverview'">
						<xsl:value-of select="@FullName"/>
						<xsl:value-of select="' '"/>
						<xsl:call-template name="gettypetype"/>
					</xsl:when>
					<xsl:when test="$show='members' and $membertype='All'">
						<xsl:value-of select="@FullName"/>: Members
					</xsl:when>
					<xsl:when test="$show='members'">
						<xsl:value-of select="@FullName"/>: <xsl:value-of select="$membertype"/> Members
					</xsl:when>
					<xsl:when test="$show='member'">
						<xsl:choose>
						<xsl:when test="$membertype='Operator'">
							<xsl:value-of select="@FullName"/>
							<xsl:value-of select="' '"/> <!-- hard space -->
							<xsl:value-of select="substring-after(Members/Member[MemberType='Method'][position()=$index+1]/@MemberName, 'op_')"/>
						</xsl:when>
						<xsl:when test="$membertype='Constructor'">
							<xsl:value-of select="@FullName"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@FullName"/>.<xsl:value-of select="Members/Member[MemberType=$membertype][position()=$index+1]/@MemberName"/>
						</xsl:otherwise>
						</xsl:choose>
						<xsl:value-of select="' '"/>
						<xsl:value-of select="$membertype"/>
					</xsl:when>

					<xsl:when test="$show='namespace'">
						<xsl:value-of select="$namespace"/> Namespace
					</xsl:when>
					
					<xsl:when test="$show='overloads'">
						<xsl:value-of select="@FullName"/>.<xsl:value-of select="$index"/> Overloads
					</xsl:when>

				</xsl:choose>
			</h3>
			</td>
			</tr>
		</table>

		<!-- SELECT WHAT TYPE OF VIEW:
				typeoverview
				members
				member
				-->
		<div class="MainArea">
		<xsl:choose>
		<xsl:when test="$show='masteroverview'">
		
			<xsl:for-each select="namespace">
				<xsl:sort select="@ns"/>
				
				<!-- Don't display the namespace if it is a sub-namespace of another one.
				     But don't consider namespaces without periods, e.g. 'System', to be
					 parent namespaces because then most everything will get grouped under it. -->
				<xsl:variable name="ns" select="@ns"/>
				<xsl:if test="count(parent::*/namespace[not(substring-before(@ns, '.')='') and starts-with($ns, concat(@ns, '.'))])=0">

				<p class="MasterNamespaceLink">
					<b><a href="N:{@ns}"><xsl:value-of select="@ns"/></a></b>
				</p>
				<blockquote class="MasterNamespaceDescription">
					<div>
					<xsl:apply-templates select="summary" mode="notoppara"/>
					</div>
					
					<!-- Display the sub-namespaces of this namespace -->
					<xsl:if test="not(substring-before($ns, '.')='')">
					<xsl:for-each select="parent::*/namespace[starts-with(@ns, concat($ns, '.'))]">
						<br/>
						<div><a href="N:{@ns}"><xsl:value-of select="@ns"/></a></div>
						<div><xsl:apply-templates select="summary" mode="notoppara"/></div>						
					</xsl:for-each>
					</xsl:if>
				</blockquote>
				
				</xsl:if>
			</xsl:for-each>
			
		</xsl:when>
		<!-- TYPE OVERVIEW -->
		<xsl:when test="$show='typeoverview'">
			<xsl:apply-templates select="Docs/since" />
			
			<!-- summary -->
			<p>
				<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
				<xsl:apply-templates select="Docs/summary" mode="editlink"/>
			</p>

			<xsl:variable name="MonoImplInfo" select="monodoc:MonoImpInfo(string(AssemblyInfo/AssemblyName), string(@FullName), true())"/>
			<xsl:if test="$MonoImplInfo">
				<p><b>Mono Implementation Note: </b>
					<blockquote>
					<xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/>
					</blockquote>
				</p>
				<br/>
			</xsl:if>

			<xsl:if test="not(Base/BaseTypeName='System.Enum' or Base/BaseTypeName='System.Delegate' or Base/BaseTypeName='System.MulticastDelegate') and count(Members)">
				<p>
				See Also: <a>
					<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/*</xsl:attribute>
					<xsl:value-of select="@Name"/> Members</a>
				</p>
			</xsl:if>
			
			<!--
			Inheritance tree, but only for non-standard classes and not for interfaces
			-->
			<xsl:if test="not(Base/BaseTypeName='System.Enum' or Base/BaseTypeName='System.Delegate' or Base/BaseTypeName='System.ValueType' or Base/BaseTypeName='System.Object' or Base/BaseTypeName='System.MulticatDelegate' or count(Base/ParentType)=0)">
				<p>
				<xsl:for-each select="Base/ParentType">
					<xsl:sort select="@Order" order="descending"/>
					<xsl:variable name="p" select="position()" />
					<xsl:for-each select="parent::Base/ParentType[position() &lt; $p]">
						<xsl:value-of select="'&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;'" disable-output-escaping="yes"/>
					</xsl:for-each>
					<a href="T:{@Type}"><xsl:value-of select="@Type"/></a>
					<br/>
				</xsl:for-each>

				<xsl:for-each select="Base/ParentType">
					<xsl:value-of select="'&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;'" disable-output-escaping="yes"/>
				</xsl:for-each>
				<xsl:value-of select="@FullName"/>
				</p>
			</xsl:if>
			<!--
			<xsl:if test="Base/BaseTypeName='System.Enum'">
				<br/>
				The type of the values in this enumeration is 
				<xsl:apply-templates select="Members/Member[@MemberName='value__']/ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>.
			</xsl:if>
			-->

			<!-- signature -->
			<table class="SignatureTable" bgcolor="#c0c0c0" cellspacing="0" width="100%">
			<tr><td>
				<table class="InnerSignatureTable" cellpadding="10" cellspacing="0" width="100%">
				<tr bgcolor="#f2f2f2"><td>

					<xsl:choose>
					<xsl:when test="$language='C#'">
						<xsl:for-each select="Attributes/Attribute">
							[<xsl:value-of select="AttributeName"/>]<br/>
						</xsl:for-each>
	
						<xsl:choose>

						<xsl:when test="Base/BaseTypeName='System.Enum'">
							<xsl:call-template name="getmodifiers">
								<xsl:with-param name="sig" select="TypeSignature[@Language='C#']/@Value"/>
							</xsl:call-template>

							enum
	
							<!-- member name, argument list -->
							<b>
							<xsl:value-of select="@Name"/>
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
							<xsl:value-of select="@Name"/>
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
		
							<b><xsl:value-of select="@Name"/></b>
		
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

			<xsl:call-template name="DisplayDocsInformation"/>
			
			<!-- requirements -->

			<h4>Requirements</h4>
			<blockquote>
			<b>Namespace: </b>
				<xsl:value-of select="$TypeNamespace"/> <br/>
			<b>Assembly: </b>
				<xsl:value-of select="AssemblyInfo/AssemblyName"/> <xsl:value-of select="concat(' ', AssemblyInfo/AssemblyVersion)"/> (in <xsl:value-of select="AssemblyInfo/AssemblyName"/>.dll) <br/>
			<b>Culture: </b>
				<xsl:value-of select="AssemblyInfo/AssemblyCulture"/> <br/>
			</blockquote>

		</xsl:when>

		<!-- MEMBER LISTING -->
		<xsl:when test="$show='members'">
			<xsl:if test="$membertype='All'">
				<p>
					The members of <xsl:value-of select="@FullName"/> are listed below.
				</p>

				<xsl:if test="Base/BaseTypeName">
					<p>
						See Also: <a>
					<xsl:attribute name="href">T:<xsl:value-of select="Base/BaseTypeName"/>/*</xsl:attribute>
					Inherited members</a> from <xsl:value-of select="Base/BaseTypeName"/>
					</p>
				</xsl:if>

				<p>
					<center>
					<table class="TypeMemebersListTable" border="0" cellpadding="6">
					<tr>
					<td>[</td>
					<xsl:if test="count(Members/Member[MemberType='Constructor'])">
						<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/C</xsl:attribute>
							Constructors</a>
						</td>
					</xsl:if>
					<xsl:if test="count(Members/Member[MemberType='Field'])">
						<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/F</xsl:attribute>
							Fields</a>
						</td>
					</xsl:if>
					<xsl:if test="count(Members/Member[MemberType='Property'])">
						<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/P</xsl:attribute>
							Properties</a>
						</td>
					</xsl:if>
					<xsl:if test="count(Members/Member[MemberType='Method' and not(starts-with(@MemberName,'op_'))])">
						<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/M</xsl:attribute>
							Methods</a>
						</td>
					</xsl:if>
					<xsl:if test="count(Members/Member[MemberType='Event'])">
						<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/E</xsl:attribute>
							Events</a>
						</td>
					</xsl:if>
					<xsl:if test="count(Members/Member[MemberType='Method' and starts-with(@MemberName,'op_')])">
						<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/E</xsl:attribute>
							Events</a>
						</td>
					</xsl:if>
					<td>]</td>					
					</tr>
					</table>
					</center>
				</p>

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
			</xsl:if>

			<xsl:if test="not($membertype='All')">
				<!-- list the members of this type (public, then protected) -->

				<p>
					The
					<xsl:call-template name="membertypeplurallc"><xsl:with-param name="name" select="$membertype"/></xsl:call-template>
					of <xsl:value-of select="@FullName"/> are listed below.  For a list of all members, see the <a>
					<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/*</xsl:attribute>
					<xsl:value-of select="@Name"/> Members</a> list.
				</p>
				
				<xsl:if test="Base/BaseTypeName">
					<p>
						See Also: <a>
					<xsl:attribute name="href">T:<xsl:value-of select="Base/BaseTypeName"/>/*</xsl:attribute>
					Inherited members</a> from <xsl:value-of select="Base/BaseTypeName"/>
					</p>
				</xsl:if>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="$membertype"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="$membertype"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>
			</xsl:if>

		</xsl:when>
		
		<xsl:when test="$show='overloads'">
				<p>
					The overloads of <xsl:value-of select="$index"/>
					are listed below.  For a list of all members, see the <a>
					<xsl:attribute name="href">T:<xsl:value-of select="@FullName"/>/*</xsl:attribute>
					<xsl:value-of select="@Name"/> Members</a> list.
				</p>
				
				<!-- TODO: can we make this actually test if there are any overloads
				<xsl:if test="Base/BaseTypeName">
					<p>
						See Also: <a>
					<xsl:attribute name="href">T:<xsl:value-of select="Base/BaseTypeName"/>/*</xsl:attribute>
					Inherited members</a> from <xsl:value-of select="Base/BaseTypeName"/>
					</p>
				</xsl:if>
				 -->
				 
				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="$membertype"/>
					<xsl:with-param name="showprotected" select="false()"/>
				</xsl:call-template>

				<xsl:call-template name="ListMembers">
					<xsl:with-param name="listmembertype" select="$membertype"/>
					<xsl:with-param name="showprotected" select="true()"/>
				</xsl:call-template>
		</xsl:when>
		<!-- MEMBER DETAILS -->
		<xsl:when test="$show='member'">
			<xsl:variable name="Type" select="."/>

			<!-- select the member, this just loops through the one member that we are to display -->
			<xsl:for-each select="Members/Member[MemberType=$membertype or ($membertype='Operator' and MemberType='Method')][position()=$index+1]">

				<!-- summary -->
				
				<xsl:variable name="since" select="Docs/since" />

				<xsl:if test="$since">
					<xsl:apply-templates select="Docs/since" />
				</xsl:if>
				<xsl:if test="not($since)">
					<xsl:apply-templates select="/Type/Docs/since" />
				</xsl:if>
				
				<p>
					<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
					<xsl:apply-templates select="Docs/summary" mode="editlink"/>
				</p>

				<xsl:variable name="MonoImplInfo" select="monodoc:MonoImpInfo(string(ancestor::Type/AssemblyInfo/AssemblyName), string(ancestor::Type/@FullName), string(@MemberName), Parameters/Parameter/@Type, true())"/>
				<xsl:if test="$MonoImplInfo">
					<p><b>Mono Implementation Note: </b>
						<blockquote>
						<xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/>
						</blockquote>
					</p>
					<br/>
				</xsl:if>

				<!-- member value -->

				<xsl:if test="MemberValue">
				<p><b>Value: </b>
					<xsl:value-of select="MemberValue"/>
				</p>
				</xsl:if>

				<!-- signature -->

				<table class="MemberSignatureTable" bgcolor="#c0c0c0" cellspacing="0" width="100%">
				<tr><td>
					<table class="MemberSignatureInnerTable" cellpadding="10" cellspacing="0" width="100%">
					<tr bgcolor="#f2f2f2"><td>

						<xsl:choose>
						<xsl:when test="$language='C#'">

						<xsl:if test="contains(MemberSignature[@Language='C#']/@Value,'this[')">
							<p><i>This is the default property for this class.</i></p>
						</xsl:if>
	
						<xsl:for-each select="Attributes/Attribute">
							[<xsl:value-of select="AttributeName"/>]<br/>
						</xsl:for-each>	
					
						<!-- recreate the signature -->

						<xsl:call-template name="getmodifiers">
							<xsl:with-param name="sig" select="MemberSignature[@Language='C#']/@Value"/>
						</xsl:call-template>

						<xsl:if test="$membertype = 'Event'">
							event

							<xsl:if test="ReturnValue/ReturnType=''">
								<xsl:value-of select="substring-before(substring-after(MemberSignature[@Language='C#']/@Value, 'event '), concat(' ', @MemberName))"/>
							</xsl:if>
						</xsl:if>

						<!-- return value (comes out "" where not applicable/available) -->
						<xsl:choose>
						<xsl:when test="@MemberName='op_Implicit' or @MemberName='op_Explicit'">
							<!-- 'operator' and 'implcit'/'explicit' already appear from the attributes list -->
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
						</xsl:otherwise>					
						</xsl:choose>

						<!-- hard space -->
						<xsl:value-of select="' '"/>

						<!-- member name -->
						<xsl:choose>
						
						<!-- Constructors get the name of the class -->
						<xsl:when test="MemberType='Constructor'">
							<b><xsl:value-of select="$Type/@Name"/></b>
						</xsl:when>
						
						<!-- Conversion operators get the return type -->
						<xsl:when test="@MemberName='op_Implicit' or @MemberName='op_Explicit'">
							<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
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
							{
							<xsl:value-of select="substring-before(substring-after(MemberSignature[@Language='C#']/@Value, '{'), '}')"/>
							}
						</xsl:if>

	
						</xsl:when>
	
						<xsl:otherwise>
							<!-- Languages other than C# -->
							<xsl:apply-templates select="MemberSignature[@Language=$language]/@Value"/>					
						</xsl:otherwise>
						
						</xsl:choose>
					</td></tr>
					</table>
				</td></tr>
				</table>
	
				<br/>

				<xsl:call-template name="DisplayDocsInformation"/>

			</xsl:for-each>

		</xsl:when>

		<!-- NAMESPACE SUMMARY -->
		<xsl:when test="$show='namespace'">

			<!-- summary -->

			<p>
				<xsl:apply-templates select="summary" mode="notoppara"/>
				<xsl:if test="monodoc:MonoEditing()">
					<xsl:value-of select="' '" />
					<a href="{monodoc:EditUrlNamespace ($namespace, 'summary')}">[Edit]</a>
				</xsl:if>
			</p>

			<!-- remarks -->

			<xsl:if test="not(remarks = '')">
				<h4>Remarks</h4>
				<blockquote>
				<xsl:apply-templates select="remarks"/>
				<xsl:if test="monodoc:MonoEditing()">
					<xsl:value-of select="' '" />
					<a href="{monodoc:EditUrlNamespace ($namespace, 'remarks')}">[Edit]</a>
				</xsl:if>
				</blockquote>
			</xsl:if>
		
			<xsl:call-template name="namespacetypes">
				<xsl:with-param name="typetype" select="'class'"/>
				<xsl:with-param name="typetitle" select="'Classes'"/>
			</xsl:call-template>

			<xsl:call-template name="namespacetypes">
				<xsl:with-param name="typetype" select="'interface'"/>
				<xsl:with-param name="typetitle" select="'Interfaces'"/>
			</xsl:call-template>

			<xsl:call-template name="namespacetypes">
				<xsl:with-param name="typetype" select="'struct'"/>
				<xsl:with-param name="typetitle" select="'Structs'"/>
			</xsl:call-template>

			<xsl:call-template name="namespacetypes">
				<xsl:with-param name="typetype" select="'delegate'"/>
				<xsl:with-param name="typetitle" select="'Delegates'"/>
			</xsl:call-template>

			<xsl:call-template name="namespacetypes">
				<xsl:with-param name="typetype" select="'enum'"/>
				<xsl:with-param name="typetitle" select="'Enumerations'"/>
			</xsl:call-template>

			
		</xsl:when>

		<!-- don't know what kind of page this is -->
		<xsl:otherwise>
			Don't know what to do!
		</xsl:otherwise>

		</xsl:choose>
		</div>
		
		<!-- FOOTER -->
		
		<div class="Footer">
		<p></p>

		<hr/>
		<p>
			This documentation is part of the <a target="_top" title="Mono Project" href="http://www.mono-project.com/">Mono Project</a>.
		</p>
		</div>

	</xsl:template>

	<xsl:template name="ShowParameter">
		<xsl:param name="Param"/>
		<xsl:param name="TypeNamespace"/>
		<xsl:param name="prototype" select="false()"/>

		<xsl:if test="not($prototype)">
			<xsl:for-each select="$Param/Attributes/Attribute[not(Exclude='1') and not(AttributeName='ParamArrayAttribute' or AttributeName='System.ParamArray')]">
				[<xsl:value-of select="AttributeName"/>]
				<xsl:value-of select="' '"/>
			</xsl:for-each>
		</xsl:if>

		<xsl:if test="count($Param/Attributes/Attribute/AttributeName[.='ParamArrayAttribute' or .='System.ParamArray'])">
			<b>params</b> <xsl:value-of select="' '"/>
		</xsl:if>

		<xsl:if test="$Param/@RefType">
			<i><xsl:value-of select="$Param/@RefType"/></i>
			<!-- hard space -->
			<xsl:value-of select="' '"/>
		</xsl:if>

		<!-- parameter type link -->
		<xsl:apply-templates select="$Param/@Type" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>

		<xsl:if test="not($prototype)">
			<!-- hard space -->
			<xsl:value-of select="' '"/>
	
			<!-- parameter name -->
			<xsl:value-of select="$Param/@Name"/>
		</xsl:if>
	</xsl:template>

	<xsl:template name="DisplayDocsInformation">
		<!-- alt member: not sure what these are for, actually -->

		<xsl:if test="count(Docs/altmember)">
			<h4>See Also</h4>
			<blockquote>
			<xsl:for-each select="Docs/altmember">
				<xsl:apply-templates select="@cref" mode="cref"/><br/>
			</xsl:for-each>
			</blockquote>
		</xsl:if>

		<!-- parameters & return & value -->

		<xsl:if test="count(Docs/typeparam)">
			<h4>Type Parameters</h4>
			<blockquote>
			<dl>
			<xsl:for-each select="Docs/typeparam">
				<dt><i><xsl:value-of select="@name"/></i></dt>
				<dd>
					<xsl:apply-templates select="." mode="notoppara"/>
					<xsl:apply-templates select="." mode="editlink"/>
				</dd>
			</xsl:for-each>
			</dl>
			</blockquote>
		</xsl:if>
		<xsl:if test="count(Docs/param)">
			<h4>Parameters</h4>
			<blockquote>
			<dl>
			<xsl:for-each select="Docs/param">
				<dt><i><xsl:value-of select="@name"/></i></dt>
				<dd>
					<xsl:apply-templates select="." mode="notoppara"/>
					<xsl:apply-templates select="." mode="editlink"/>
				</dd>
			</xsl:for-each>
			</dl>
			</blockquote>
		</xsl:if>
		<xsl:if test="count(Docs/returns)">
			<h4>Returns</h4>
			<blockquote>
				<xsl:apply-templates select="Docs/returns" mode="notoppara"/>
				<xsl:apply-templates select="Docs/returns" mode="editlink"/>
			</blockquote>
		</xsl:if>
		<xsl:if test="count(Docs/value)">
			<h4>Value</h4>
			<blockquote>
				<xsl:apply-templates select="Docs/value" mode="notoppara"/>
				<xsl:apply-templates select="Docs/value" mode="editlink"/>
			</blockquote>
		</xsl:if>

		<!-- thread safety -->

		<xsl:if test="count(ThreadingSafetyStatement)">
			<h4>Thread Safety</h4>
			<blockquote>
			<xsl:apply-templates select="ThreadingSafetyStatement" mode="notoppara"/>
			</blockquote>
		</xsl:if>


		<!-- permissions -->

		<xsl:if test="count(Docs/permission)">
			<h4>Permissions</h4>
			<blockquote>
			<table class="TypePermissionsTable" border="1" cellpadding="6">
			<tr bgcolor="#f2f2f2"><th>Type</th><th>Reason</th></tr>
			<xsl:for-each select="Docs/permission">
				<tr valign="top">
				<td><xsl:apply-templates select="@cref" mode="typelink"><xsl:with-param name="wrt" select="'System.Security.Permissions'"/></xsl:apply-templates></td>
				<td>
					<xsl:apply-templates select="." mode="notoppara"/>
				</td>
				</tr>
			</xsl:for-each>
			</table>
			</blockquote>
		</xsl:if>

		<!-- method/property/constructor exceptions -->

		<xsl:if test="count(Docs/exception)">
			<h4>Exceptions</h4>
			<blockquote>
			<table class="ExceptionsTable" border="1" cellpadding="6">
			<tr bgcolor="#f2f2f2"><th>Type</th><th>Condition</th></tr>
			<xsl:for-each select="Docs/exception">
				<tr valign="top">
				<td><xsl:apply-templates select="@cref" mode="typelink"><xsl:with-param name="wrt" select="'System'"/></xsl:apply-templates></td>
				<td>
					<xsl:apply-templates select="." mode="notoppara"/>
				</td>
				</tr>
			</xsl:for-each>
			</table>
			</blockquote>
		</xsl:if>

		<!-- remarks -->

		<xsl:if test="count(Docs/remarks)">
			<h4>Remarks</h4>
			<blockquote>
			<xsl:apply-templates select="Docs/remarks" mode="notoppara"/>
			<xsl:apply-templates select="Docs/remarks" mode="editlink"/>
			<br />
			</blockquote>
		</xsl:if>

		<!-- enumeration values -->

		<xsl:if test="Base/BaseTypeName = 'System.Enum'">
			<h4>Members</h4>
			<blockquote>
			<table class="EnumerationsTable" border="1" cellpadding="10" width="100%">
			<tr bgcolor="#f2f2f2">
				<th>Member Name</th>
				<th>Description</th>
			</tr>

			<xsl:choose>
			<!-- The editing stuff is fairly slow, so limit it to small cases -->
				<xsl:when test="count (Members/Member[MemberType='Field']) &lt;= 50">
					<xsl:for-each select="Members/Member[MemberType='Field']">
						<xsl:if test="not(@MemberName='value__')">
							<tr><td><b>
								<xsl:value-of select="@MemberName"/>
							</b></td>
							<td>
								<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
								<xsl:text> </xsl:text>
								<xsl:apply-templates select="Docs/summary" mode="editlink"/>
							</td>
							</tr>
						</xsl:if>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
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
				</xsl:otherwise>
			</xsl:choose>


			</table>
			</blockquote>
		</xsl:if>

		<!-- examples -->

		<xsl:if test="count(Docs/example)">
			<h4>Examples</h4>
			<blockquote>
			<xsl:for-each select="Docs/example">
				<xsl:apply-templates select="." mode="notoppara"/>
			</xsl:for-each>
			</blockquote>
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
		<xsl:param name="type" select="'notset'"/>
		<xsl:param name="wrt" select="'notset'"/>
		<xsl:param name="nested" select="0"/>

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

		<xsl:when test="contains($type, '+') and not($nested)">
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="substring-before($type, '+')"/>
				<xsl:with-param name="wrt" select="$wrt"/>
			</xsl:call-template>
			<xsl:value-of select="'.'"/>
			<xsl:call-template name="maketypelink">
				<xsl:with-param name="type" select="$type"/>
				<xsl:with-param name="wrt" select="$wrt"/>
				<xsl:with-param name="nested" select="1"/>
			</xsl:call-template>
		</xsl:when>

		<xsl:otherwise>
			
			<xsl:variable name="T" select="$type"/>
			
			<a>
				<xsl:attribute name="href">T:<xsl:value-of select="$T"/></xsl:attribute>
	
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
					<xsl:when test="not($wrt='') and starts-with($T, concat($wrt,'.')) and not(contains(substring-after($T, concat($wrt,'.')), '.'))">
						<xsl:value-of select="substring-after($T, concat($wrt,'.'))"/>
					</xsl:when>
	
					<xsl:otherwise>
						<xsl:value-of select="monodoc:MakeNiceSignature(concat('T:', $T), string(/Type/@FullName))"/>										
					</xsl:otherwise>
				</xsl:choose>
			</a>
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- Transforms the contents of the selected node into a hyperlink to the member named by the node. -->
	<xsl:template match="*|@*" mode="memberlink">
		<xsl:call-template name="memberlinkprefix"/>
		<xsl:value-of select="':'"/>
		<xsl:choose>
		 	<!-- @DelcaredIn is inserted by ecma-provider.cs at runtime -->
			<xsl:when test="count(@DeclaredIn)"><xsl:value-of select="@DeclaredIn"/></xsl:when>
			<xsl:otherwise><xsl:value-of select="ancestor::Type/@FullName"/></xsl:otherwise>
		</xsl:choose>
		<xsl:if test="not(MemberType='Constructor')">.<xsl:value-of select="@MemberName"/></xsl:if>
		<xsl:choose>
		<xsl:when test="MemberType='Constructor' or MemberType='Method' or (MemberType='Property' and count(Parameters/Parameter) &gt; 0)">
			<xsl:value-of select="'('"/>
			<xsl:for-each select="Parameters/Parameter">
				<xsl:if test="not(position()=1)">,</xsl:if>
				<xsl:value-of select="@Type"/>
			</xsl:for-each>
			<xsl:if test="@MemberName = 'op_Implicit' or @MemberName = 'op_Explicit'">
				<xsl:value-of select="':'"/>
				<xsl:value-of select="ReturnValue/ReturnType"/>
			</xsl:if>
			<xsl:value-of select="')'"/>
		</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="memberlinkprefix">
		<xsl:choose>
			<xsl:when test="MemberType='Constructor'">C</xsl:when>
			<xsl:when test="MemberType='Method'">M</xsl:when>
			<xsl:when test="MemberType='Property'">P</xsl:when>
			<xsl:when test="MemberType='Field'">F</xsl:when>
			<xsl:when test="MemberType='Event'">F</xsl:when>
		</xsl:choose>
	</xsl:template>

	<!-- Transforms the contents of the selected node containing a cref into a hyperlink. -->
	<xsl:template match="*|@*" mode="cref">
		<a>
			<xsl:attribute name="href"><xsl:value-of select="."/></xsl:attribute>
			<xsl:value-of select="substring-after(., ':')"/></a>
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
		<xsl:when test="monodoc:IsToBeAdded(string(.))">
			Documentation for this section has not yet been entered.
		</xsl:when>
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
		<p><h4>Operation</h4><xsl:apply-templates/></p>
	</xsl:template>
	<xsl:template match="block[@type='overrides']">
		<p><h4>Note to Inheritors</h4><xsl:apply-templates/></p>
	</xsl:template>
	<xsl:template match="block[@type='usage']">
		<p><h4>Usage</h4><xsl:apply-templates/></p>
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
		<table class="CodeExampleTable" bgcolor="#f5f5dd" border="1" cellpadding="5">
		<tr><td><b><font size="-1"><xsl:value-of select="@lang"/> Example</font></b></td></tr>
		<tr><td><font size="-1">
			<xsl:value-of select="monodoc:Colorize(string(descendant-or-self::text()), string(@lang))" disable-output-escaping="yes" />
		</font></td></tr>
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
		<tr bgcolor="#f2f2f2" valign="top">
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
		<UL>
			<xsl:for-each select="item">
				<LI>
					<xsl:apply-templates select="term" mode="notoppara"/>
				</LI>
			</xsl:for-each>
		</UL>
	</xsl:template>
	<xsl:template match="list[@type='number']">
		<OL>
			<xsl:for-each select="item">
				<LI>
					<xsl:apply-templates select="term" mode="notoppara"/>
				</LI>
			</xsl:for-each>
		</OL>
	</xsl:template>

	<xsl:template match="list">
		[<i>The '<xsl:value-of select="@type"/>' type of list has not been implemented in the ECMA stylesheet.</i>]
		
		<xsl:message>
		[<i>The '<xsl:value-of select="@type"/>' type of list has not been implemented in the ECMA stylesheet.</i>]
		</xsl:message>
	</xsl:template>

	<xsl:template match="see[@cref]">
		<a href="{@cref}">
			<xsl:if test="$show='namespace'">
				<xsl:value-of select="monodoc:MakeNiceSignature(string(@cref), concat($namespace,'.DUMMYTYPENAME'))"/>
			</xsl:if>
			<xsl:if test="not($show='namespace')">
				<xsl:value-of select="monodoc:MakeNiceSignature(string(@cref), string(/Type/@FullName))"/>
			</xsl:if>
		</a>
	</xsl:template>

	<xsl:template match="see[@langword]">
		<!--<a href="{@langword}"><xsl:value-of select="@langword"/></a>-->
		<tt><xsl:value-of select="@langword"/></tt>
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
		<xsl:variable name="TypeName" select="@Name"/>		
		<xsl:variable name="TypeNamespace" select="substring-before(@FullName, concat('.',@Name))"/>

		<!-- Get all members in this type that are of listmembertype and are either
			protected or not protected according to showprotected. -->
		<xsl:variable name="MEMBERS" select="
			Members/Member
			[(MemberType=$listmembertype or ($listmembertype='Operator' and MemberType='Method'))] [(not($show='overloads') or @MemberName=$index or ($index='Conversion' and (@MemberName='op_Implicit' or @MemberName='op_Explicit'))) ]
			[$showprotected=contains(MemberSignature[@Language='C#']/@Value,'protected')]
			[($listmembertype='Method' and not(starts-with(@MemberName,'op_')))
				or ($listmembertype='Operator' and starts-with(@MemberName,'op_'))
				or (not($listmembertype='Method') and not($listmembertype='Operator'))]
			"/>

		<!-- if there aren't any, skip this -->
		<xsl:if test="count($MEMBERS)">

		<!-- header -->
		<p>
		<h4>
			<xsl:if test="$showprotected">Protected </xsl:if>
			<xsl:call-template name="membertypeplural"><xsl:with-param name="name" select="$listmembertype"/></xsl:call-template>
			</h4>

		<blockquote>
		<table border="1" cellpadding="6" width="100%">

		<xsl:for-each select="$MEMBERS">
			<!--<xsl:sort select="contains(MemberSignature[@Language='C#']/@Value,' static ')" data-type="text"/>-->
			<xsl:sort select="@MemberName = 'op_Implicit' or @MemberName = 'op_Explicit'"/>
			<xsl:sort select="@MemberName" data-type="text"/>
			<xsl:sort select="count(Parameters/Parameter)"/>
			<xsl:sort select="Parameters/Parameter/@Type"/>

			<xsl:variable name="MonoImplInfo" select="monodoc:MonoImpInfo(string(ancestor::Type/AssemblyInfo/AssemblyName), string(ancestor::Type/@FullName), string(@MemberName), Parameters/Parameter/@Type, false())"/>
	
			<tr valign="top">

			<xsl:choose>
				<!-- constructor listing -->
				<xsl:when test="MemberType='Constructor'">
					<!-- link to constructor page -->
					<td>
					<b>
					<a>
						<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>
						<xsl:value-of select="$TypeName"/>
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
					<br/>

					<!-- description -->
					<xsl:apply-templates select="Docs/summary"  mode="notoppara"/>

					<xsl:if test="$MonoImplInfo"> (<I><xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/></I>)</xsl:if>

					</td>
				</xsl:when>

				<!-- field, property and event listing -->
				<xsl:when test="MemberType='Field' or MemberType='Property' or MemberType='Event'">
					<td>

					<!-- link to member page -->
					<b>
					<a>
						<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>
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

					<br/>
					<xsl:if test="contains(MemberSignature[@Language='C#']/@Value,'this[')">
						<i>default property</i><br/>
					</xsl:if>
					
					<xsl:if test="count(@DeclaredIn)">
						<i>inherited</i><br/>
					</xsl:if>				

					<xsl:call-template name="getmodifiers">
						<xsl:with-param name="sig" select="MemberSignature[@Language='C#']/@Value"/>
						<xsl:with-param name="protection" select="false()"/>
						<xsl:with-param name="inheritance" select="true()"/>
						<xsl:with-param name="extra" select="false()"/>
					</xsl:call-template>

					</td>
					<td>
					<!-- field/property type -->
					<xsl:if test="not(MemberType='Event')">
						<i><xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates></i>
						<xsl:if test="MemberValue">
						<xsl:value-of select="' '"/>
						(<xsl:value-of select="MemberValue"/>)
						</xsl:if>.
						<xsl:value-of select="' '"/>
					</xsl:if>

					<!-- description -->
					<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
					<xsl:if test="$MonoImplInfo"> (<I><xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/></I>)</xsl:if>
					</td>
				</xsl:when>

				<!-- method listing -->
				<xsl:when test="$listmembertype='Method'">
					<td>
						<xsl:if test="count(@DeclaredIn)">
							<i>inherited</i><br/>
						</xsl:if>
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
					<a>
						<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>
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
						:
						<xsl:apply-templates select="ReturnValue/ReturnType" mode="typelink"><xsl:with-param name="wrt" select="$TypeNamespace"/></xsl:apply-templates>
						</nobr>
					</xsl:if>

					<!-- description -->
					<blockquote>
						<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
						<xsl:if test="$MonoImplInfo"> (<I><xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/></I>)</xsl:if>
					</blockquote>
					</td>
				</xsl:when>

				<xsl:when test="$listmembertype='Operator'">
					<td>

					<!-- link to operator page -->
					<xsl:choose>
					<xsl:when test="@MemberName='op_Implicit' or @MemberName='op_Explicit'">
						<b>
						<a>
							<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>	
							Conversion
							<xsl:choose>
							<xsl:when test="ReturnValue/ReturnType = //Type/@FullName">
								From <xsl:value-of select="Parameters/Parameter/@Type"/>
							</xsl:when>
							<xsl:otherwise>
								to <xsl:value-of select="ReturnValue/ReturnType"/>
							</xsl:otherwise>
							</xsl:choose>
						</a>
						</b>						

						<xsl:choose>
						<xsl:when test="@MemberName='op_Implicit'">
							(Implicit)
						</xsl:when>
						<xsl:otherwise>
							(Explicit)
						</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="count(Parameters/Parameter)=1">
						<b>
						<a>
							<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>	
							<xsl:value-of select="substring-after(@MemberName, 'op_')"/>
						</a>
						</b>
					</xsl:when>
					<xsl:otherwise>
						<b>
						<a>
							<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>	
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
					<blockquote>
						<xsl:apply-templates select="Docs/summary" mode="notoppara"/>
						<xsl:if test="$MonoImplInfo"> (<I><xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/></I>)</xsl:if>
					</blockquote>
					</td>
				</xsl:when>
				
				<xsl:otherwise>
					<!-- Other types: just provide a link -->
					<a>
						<xsl:attribute name="href"><xsl:apply-templates select="." mode="memberlink"/></xsl:attribute>
						<xsl:value-of select="@MemberName"/>
					</a>
				</xsl:otherwise>
			</xsl:choose>
			
			<!--<tt><xsl:value-of select="MemberSignature[@Language='C#']/@Value"/></tt><br/>-->

			</tr>
		</xsl:for-each>

		</table>
		</blockquote>
		</p>

		</xsl:if>
	</xsl:template>

	<xsl:template name="namespacetypes">
		<xsl:param name="typetype"/>
		<xsl:param name="typetitle"/>

		<xsl:variable name="NODES" select="*[name()=$typetype]"/>

		<xsl:if test="count($NODES)">

		<p>
		<h4><xsl:value-of select="$typetitle"/></h4>

		<blockquote>
		
		<table border="1" cellpadding="6">
			<tr><th>Type</th><th>Summary</th></tr>

			<xsl:for-each select="$NODES">
				<xsl:sort select="@name"/>

				<tr valign="top">
					<td>
						<a>
							<xsl:attribute name="href">T:<xsl:value-of select="@fullname"/></xsl:attribute>
							<xsl:value-of select="@name"/>
						</a>

						<xsl:variable name="containingtype" select="substring-before(@fullname, concat('+',@name))"/>
						<xsl:if test="$containingtype">
						<br/>(in
							<xsl:call-template name="maketypelink">
								<xsl:with-param name="type" select="$containingtype"/>
								<xsl:with-param name="wrt" select="$namespace"/>
							</xsl:call-template>)
						</xsl:if>
					</td>
					<td>
						<xsl:apply-templates select="summary" mode="notoppara"/>

						<xsl:variable name="MonoImplInfo" select="monodoc:MonoImpInfo(string(@assembly), string(@fullname), false())"/>
						<xsl:if test="$MonoImplInfo"><br/><b><xsl:value-of disable-output-escaping="yes" select="$MonoImplInfo"/></b></xsl:if>
					</td>
				</tr>
			</xsl:for-each>
		</table>

		</blockquote>

		</p>

		</xsl:if>
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
	
	<xsl:template match="*|@*" mode="editlink">
		<xsl:if test="monodoc:MonoEditing()">
			<xsl:value-of select="' '" />
			[<a href="{monodoc:EditUrl (.)}">Edit</a>]
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="since">
		<p>
			<i>Note: This namespace, class, or member is supported only in version <xsl:value-of select="@version" />
			and later.</i>
		</p>
	</xsl:template>

</xsl:stylesheet>
