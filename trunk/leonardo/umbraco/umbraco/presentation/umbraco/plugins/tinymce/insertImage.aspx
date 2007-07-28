<%@ Page language="c#" Codebehind="insertImage.aspx.cs" AutoEventWireup="True" Inherits="umbraco.presentation.tinymce.insertImage" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>{$lang_insert_image_title}</title>
	<script language="javascript" type="text/javascript" src="../../../umbraco_client/tinymce/tiny_mce_popup.js"></script>
	<script language="javascript" type="text/javascript" src="../../../umbraco_client/tinymce/utils/mctabs.js"></script>
	<script language="javascript" type="text/javascript" src="../../../umbraco_client/tinymce/utils/form_utils.js"></script>
	<script language="javascript" type="text/javascript" src="../../../umbraco_client/tinymce/plugins/advimage/jscripts/functions.js"></script>
	<link href="../../../umbraco_client/tinymce/plugins/advimage/css/advimage.css" rel="stylesheet" type="text/css" />
	<base target="_self" />
	<script language="javascript">
		function dialogHandler(id) {
			document.getElementById('imageViewer').src = '../../dialogs/imageViewer.aspx?id=' + id;
		}
		
		function updateImageSource(src, alt, width, height) {
			var formObj = document.forms[0];
			formObj.src.value    = src;
			formObj.alt.value    = alt;

			var imageWidth = width;
			var imageHeight = height;
			var orgHeight = height;
			var orgWidth = width;
			
			if (imageWidth != '' && imageWidth > 500) {						
					if (imageWidth > imageHeight)
						orgRatio = parseFloat(imageHeight/imageWidth).toFixed(2)
					else
						orgRatio = parseFloat(imageWidth/imageHeight).toFixed(2)
					imageHeight = Math.round(500 * parseFloat(imageHeight/imageWidth).toFixed(2));
					imageWidth = 500;												
			}

			formObj.width.value  = imageWidth;
			formObj.height.value = imageHeight;
			
			formObj.orgWidth.value = width;
			formObj.orgHeight.value = height;
		}
		
		function insertImage()
		{
			var imageName = document.getElementById("imageAlt").value;
			var imageWidth = document.getElementById("imageWidth").value;
			var imageHeight = document.getElementById("imageHeight").value;
			var imageSource = document.getElementById("imageSrc").value.replace("thumb.jpg", ".jpg");
			var imageTitle = document.getElementById("imageTitle").value;
			var orgHeight = imageHeight;
			var orgWidth = imageWidth;
			
			if (imageWidth != '' && imageWidth > 500) {						
					if (imageWidth > imageHeight)
						orgRatio = parseFloat(imageHeight/imageWidth).toFixed(2)
					else
						orgRatio = parseFloat(imageWidth/imageHeight).toFixed(2)
					imageHeight = Math.round(500 * parseFloat(imageHeight/imageWidth).toFixed(2));
					imageWidth = 500;												
			}
			window.returnValue = imageName + '|||' + imageSource + '|||' + imageWidth + '|||' + imageHeight + '|||' + imageTitle + '|||' + orgWidth + '|||' + orgHeight;
			window.close();
		}


		function doSubmit() {}

		var functionsFrame = this;
		var tabFrame = this;
		var isDialog = true;
		var submitOnEnter = true;

	</script>
</head>
<body id="advimage" onload="tinyMCEPopup.executeOnLoad('init();');" style="display: none">
    <form id="F" method="post" runat="server" onsubmit="insertAction();return false;">
    <input type="hidden" name="orgWidth" />
    <input type="hidden" name="orgHeight" />
		<div class="tabs">
			<ul>
				<li id="general_tab" class="current"><span><a href="javascript:mcTabs.displayTab('general_tab','general_panel');" onmousedown="return false;">{$lang_advimage_tab_general}</a></span></li>
				<li id="appearance_tab"><span><a href="javascript:mcTabs.displayTab('appearance_tab','appearance_panel');" onmousedown="return false;">{$lang_advimage_tab_appearance}</a></span></li>
				<li id="advanced_tab"><span><a href="javascript:mcTabs.displayTab('advanced_tab','advanced_panel');" onmousedown="return false;">{$lang_advimage_tab_advanced}</a></span></li>
			</ul>
		</div>

		<div class="panel_wrapper" style="height: 460px;">
			<div id="general_panel" class="panel current">
				<fieldset>
					<legend><%=umbraco.ui.Text("choose") %></legend>
					<table class="propertyPane" cellSpacing="0" cellPadding="4" width="98%" border="0" runat="server" ID="Table1">
						<TBODY>
							
							<tr>
								<TD class="propertyHeader" width="30%">
									<asp:PlaceHolder ID="PlaceHolder1" Runat="server" />
								</TD>
							</tr>
							
						</TBODY>
					</table>
				</fieldset>
				
				<fieldset>
						<legend>{$lang_advimage_general}</legend>
						<table class="properties">
							<tr>
								<td class="column1"><label id="srclabel" for="src">{$lang_insert_image_src}</label></td>
								<td colspan="2"><table border="0" cellspacing="0" cellpadding="0">
									<tr> 
									  <td><input name="src" type="text" id="src" value="" onchange="showPreviewImage(this.value);" /></td> 
									  <td id="srcbrowsercontainer">&nbsp;</td>
									</tr>
								  </table></td>
							</tr>
							<tr id="imagelistsrcrow">
								<td class="column1"><label for="imagelistsrc">{$lang_image_list}</label></td>
								<td colspan="2" id="imagelistsrccontainer">&nbsp;</td>
							</tr>
							<tr> 
								<td class="column1"><label id="altlabel" for="alt">{$lang_insert_image_alt}</label></td> 
								<td colspan="2"><input id="alt" name="alt" type="text" value="" /></td> 
							</tr> 
							<tr> 
								<td class="column1"><label id="titlelabel" for="title">{$lang_advimage_title}</label></td> 
								<td colspan="2"><input id="title" name="title" type="text" value="" /></td> 
							</tr>
						</table>
				</fieldset>

				<fieldset style="display:none;">
					<legend>{$lang_advimage_preview}</legend>
					<div id="prev"></div>
				</fieldset>
			</div>

			<div id="appearance_panel" class="panel">
				<fieldset>
					<legend>{$lang_advimage_tab_appearance}</legend>

					<table border="0" cellpadding="4" cellspacing="0">
						<tr> 
							<td class="column1"><label id="alignlabel" for="align">{$lang_insert_image_align}</label></td> 
							<td><select id="align" name="align" onchange="changeAppearance();"> 
									<option value="">{$lang_insert_image_align_default}</option> 
									<option value="baseline">{$lang_insert_image_align_baseline}</option> 
									<option value="top">{$lang_insert_image_align_top}</option> 
									<option value="middle">{$lang_insert_image_align_middle}</option> 
									<option value="bottom">{$lang_insert_image_align_bottom}</option> 
									<option value="texttop">{$lang_insert_image_align_texttop}</option> 
									<option value="absmiddle">{$lang_insert_image_align_absmiddle}</option> 
									<option value="absbottom">{$lang_insert_image_align_absbottom}</option> 
									<option value="left">{$lang_insert_image_align_left}</option> 
									<option value="right">{$lang_insert_image_align_right}</option> 
								</select> 
							</td>
							<td rowspan="6" valign="top">
								<div class="alignPreview">
									<img id="alignSampleImg" src="images/sample.gif" alt="{$lang_advimage_example_img}" />
									Lorem ipsum, Dolor sit amet, consectetuer adipiscing loreum ipsum edipiscing elit, sed diam
									nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.Loreum ipsum
									edipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam
									erat volutpat.
								</div>
							</td>
						</tr>

						<tr>
							<td class="column1"><label id="widthlabel" for="width">{$lang_insert_image_dimensions}</label></td>
							<td nowrap="nowrap">
								<input name="width" type="text" id="width" value="" size="5" maxlength="5" onchange="changeHeight();" /> x 
								<input name="height" type="text" id="height" value="" size="5" maxlength="5" onchange="changeWidth();" /> px
							</td>
						</tr>

						<tr>
							<td>&nbsp;</td>
							<td><table border="0" cellpadding="0" cellspacing="0">
									<tr>
										<td><input id="constrain" type="checkbox" name="constrain" class="checkbox" /></td>
										<td><label id="constrainlabel" for="constrain">{$lang_advimage_constrain_proportions}</label></td>
									</tr>
								</table></td>
						</tr>

						<tr>
							<td class="column1"><label id="vspacelabel" for="vspace">{$lang_insert_image_vspace}</label></td> 
							<td><input name="vspace" type="text" id="vspace" value="" size="3" maxlength="3" onchange="changeAppearance();updateStyle();" />
							</td>
						</tr>

						<tr> 
							<td class="column1"><label id="hspacelabel" for="hspace">{$lang_insert_image_hspace}</label></td> 
							<td><input name="hspace" type="text" id="hspace" value="" size="3" maxlength="3" onchange="changeAppearance();updateStyle();" /></td> 
						</tr>

						<tr>
							<td class="column1"><label id="borderlabel" for="border">{$lang_insert_image_border}</label></td> 
							<td><input id="border" name="border" type="text" value="" size="3" maxlength="3" onchange="changeAppearance();updateStyle();" /></td> 
						</tr>

						<tr>
							<td><label id="classlabel" for="classlist">{$lang_class_name}</label></td>
							<td colspan="2">
								 <select id="classlist" name="classlist">
									<option value="" selected>{$lang_not_set}</option>
								 </select>
							</td>
						</tr>

						<tr>
							<td class="column1"><label id="stylelabel" for="style">{$lang_advimage_style}</label></td> 
							<td colspan="2"><input id="style" name="style" type="text" value="" onchange="styleUpdated();" /></td> 
						</tr>

						<!-- <tr>
							<td class="column1"><label id="classeslabel" for="classes">{$lang_advimage_classes}</label></td> 
							<td colspan="2"><input id="classes" name="classes" type="text" value="" onchange="selectByValue(this.form,'classlist',this.value,true);" /></td> 
						</tr> -->
					</table>
				</fieldset>
			</div>

			<div id="advanced_panel" class="panel">
				<fieldset>
					<legend>{$lang_advimage_swap_image}</legend>

					<input type="checkbox" id="onmousemovecheck" name="onmousemovecheck" class="checkbox" onclick="changeMouseMove();" />
					<label id="onmousemovechecklabel" for="onmousemovecheck">{$lang_advimage_alt_image}</label>

					<table border="0" cellpadding="4" cellspacing="0" width="100%">
							<tr>
								<td class="column1"><label id="onmouseoversrclabel" for="onmouseoversrc">{$lang_advimage_mouseover}</label></td> 
								<td><table border="0" cellspacing="0" cellpadding="0"> 
									<tr> 
									  <td><input id="onmouseoversrc" name="onmouseoversrc" type="text" value="" /></td> 
									  <td id="onmouseoversrccontainer">&nbsp;</td>
									</tr>
								  </table></td>
							</tr>
							<tr id="imagelistoverrow">
								<td class="column1"><label for="imagelistover">{$lang_image_list}</label></td>
								<td id="imagelistovercontainer">&nbsp;</td>
							</tr>
							<tr> 
								<td class="column1"><label id="onmouseoutsrclabel" for="onmouseoutsrc">{$lang_advimage_mouseout}</label></td> 
								<td class="column2"><table border="0" cellspacing="0" cellpadding="0"> 
									<tr> 
									  <td><input id="onmouseoutsrc" name="onmouseoutsrc" type="text" value="" /></td> 
									  <td id="onmouseoutsrccontainer">&nbsp;</td>
									</tr> 
								  </table></td> 
							</tr>
							<tr id="imagelistoutrow">
								<td class="column1"><label for="imagelistout">{$lang_image_list}</label></td>
								<td id="imagelistoutcontainer">&nbsp;</td>
							</tr>
					</table>
				</fieldset>

				<fieldset>
					<legend>{$lang_advimage_misc}</legend>

					<table border="0" cellpadding="4" cellspacing="0">
						<tr>
							<td class="column1"><label id="idlabel" for="id">{$lang_advimage_id}</label></td> 
							<td><input id="id" name="id" type="text" value="" /></td> 
						</tr>

						<tr>
							<td class="column1"><label id="dirlabel" for="dir">{$lang_advimage_langdir}</label></td> 
							<td>
								<select id="dir" name="dir" onchange="changeAppearance();"> 
										<option value="">{$lang_not_set}</option> 
										<option value="ltr">{$lang_advimage_ltr}</option> 
										<option value="rtl">{$lang_advimage_rtl}</option> 
								</select>
							</td> 
						</tr>

						<tr>
							<td class="column1"><label id="langlabel" for="lang">{$lang_advimage_langcode}</label></td> 
							<td>
								<input id="lang" name="lang" type="text" value="" />
							</td> 
						</tr>

						<tr>
							<td class="column1"><label id="usemaplabel" for="usemap">{$lang_advimage_image_map}</label></td> 
							<td>
								<input id="usemap" name="usemap" type="text" value="" />
							</td> 
						</tr>

						<tr>
							<td class="column1"><label id="longdesclabel" for="longdesc">{$lang_advimage_long_desc}</label></td>
							<td><table border="0" cellspacing="0" cellpadding="0">
									<tr>
									  <td><input id="longdesc" name="longdesc" type="text" value="" /></td>
									  <td id="longdesccontainer">&nbsp;</td>
									</tr>
								</table></td> 
						</tr>
					</table>
				</fieldset>
			</div>
		</div>

		<div class="mceActionPanel">
			<div style="float: left">
				<input type="button" id="insert" name="insert" value="{$lang_insert}" onclick="insertAction();" />
			</div>

			<div style="float: right">
				<input type="button" id="cancel" name="cancel" value="{$lang_cancel}" onclick="cancelAction();" />
			</div>
		</div>
    </form>
    <script>
        if (tinyMCE.settings["umbracoAdvancedMode"] != 'true') {
            document.getElementById('appearance_tab').style.visibility = 'hidden';
            document.getElementById('advanced_tab').style.visibility = 'hidden';
        }
    </script>
</body> 
</html>