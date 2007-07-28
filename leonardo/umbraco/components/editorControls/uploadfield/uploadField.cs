using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace umbraco.editorControls
{
	public class uploadField : System.Web.UI.HtmlControls.HtmlInputFile, interfaces.IDataEditor
	{
		private String _text;
		private cms.businesslogic.datatype.DefaultData _data;
        private String _thumbnails;

        public uploadField(interfaces.IData Data, string ThumbnailSizes)
        {
            _data = (cms.businesslogic.datatype.DefaultData)Data;
            _thumbnails = ThumbnailSizes;
		}

		public Control Editor {get{return this;}}

		public virtual bool TreatAsRichTextEditor 
		{
			get {return false;}
		}
		public bool ShowLabel 
		{
			get {return true;}
		}

		public String Text
		{
			get {return _text;}
			set {_text = value;}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			if (!Page.IsPostBack && _data.PropertyId != 0) {
				this.Text = _data.Value.ToString();
			}
		}

		public void Save() 
		{
				// Clear data
				if (helper.Request(this.ClientID + "clear") == "1") 
				{
					// delete file
					if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(_text)))
						System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(_text));

					// set filename in db to nothing
					_text = "";
					_data.Value = _text;
				}

				if (this.PostedFile != null) 
				{
					if (this.PostedFile.FileName != "") 
					{
						// Find filename
						_text = this.PostedFile.FileName;
						string filename; 
						string _fullFilePath;
						
						if (umbraco.UmbracoSettings.UploadAllowDirectories) 
						{
							filename = _text.Substring(_text.LastIndexOf("\\")+1, _text.Length-_text.LastIndexOf("\\")-1).ToLower();
							// Create a new folder in the /media folder with the name /media/propertyid
                            System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(umbraco.GlobalSettings.Path + "/../media/" + _data.PropertyId.ToString()));
                            _fullFilePath = System.Web.HttpContext.Current.Server.MapPath(umbraco.GlobalSettings.Path + "/../media/" + _data.PropertyId + "/" + filename);
							this.PostedFile.SaveAs(_fullFilePath);
							_data.Value = "/media/" + _data.PropertyId + "/" + filename;
						} 
						else {
							filename = _text.Substring(_text.LastIndexOf("\\")+1, _text.Length-_text.LastIndexOf("\\")-1).ToLower();
							filename = _data.PropertyId + "-" + filename;
                            _fullFilePath = System.Web.HttpContext.Current.Server.MapPath(umbraco.GlobalSettings.Path + "/../media/" + filename);
							this.PostedFile.SaveAs(_fullFilePath);
							_data.Value = "/media/" + filename;
						}
						
						// Save extension
						string orgExt = ((string)_text.Substring(_text.LastIndexOf(".")+1, _text.Length-_text.LastIndexOf(".")-1));
                        string ext = orgExt.ToLower();
						try 
						{
							cms.businesslogic.Content.GetContentFromVersion(_data.Version).getProperty("umbracoExtension").Value = ext;
						} 
						catch {}
						

						
						// Save file size
						try 
						{
							System.IO.FileInfo fi = new FileInfo(_fullFilePath);
							cms.businesslogic.Content.GetContentFromVersion(_data.Version).getProperty("umbracoBytes").Value = fi.Length.ToString();
						} 
						catch {}

						// Check if image and then get sizes, make thumb and update database
						if (",jpeg,jpg,gif,bmp,png,tiff,tif,".IndexOf(","+ext+",") > 0) 
						{
							int fileWidth;
							int fileHeight;

							FileStream fs = new FileStream(_fullFilePath, 
								FileMode.Open, FileAccess.Read, FileShare.Read);

                            System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
							fileWidth = image.Width;
							fileHeight = image.Height;
							fs.Close();
							try 
							{
								cms.businesslogic.Content.GetContentFromVersion(_data.Version).getProperty("umbracoWidth").Value = fileWidth.ToString();
								cms.businesslogic.Content.GetContentFromVersion(_data.Version).getProperty("umbracoHeight").Value = fileHeight.ToString();
							
							} 
							catch {
								
							}

                            // Generate thumbnails
                            string fileNameThumb = _fullFilePath.Replace("." + orgExt, "_thumb");
                            generateThumbnail(image, 100, fileWidth, fileHeight, _fullFilePath, ext, fileNameThumb + ".jpg");

                            if (_thumbnails != "")
                            {
                                string[] thumbnailSizes = _thumbnails.Split(";".ToCharArray());
                                foreach(string thumb in thumbnailSizes)
                                    if (thumb != "")
                                        generateThumbnail(image, int.Parse(thumb), fileWidth, fileHeight, _fullFilePath, ext, fileNameThumb + "_" + thumb + ".jpg");
                            }

                            image.Dispose();
						} 
						
					} 
					this.Text = _data.Value.ToString();
				}  
			}

        private void generateThumbnail(System.Drawing.Image image, int maxWidthHeight, int fileWidth, int fileHeight, string fullFilePath, string ext, string thumbnailFileName)
        {
            // Generate thumbnail
            float fx = (float)fileWidth / (float)maxWidthHeight;
            float fy = (float)fileHeight / (float)maxWidthHeight;
            // must fit in thumbnail size
            float f = Math.Max(fx, fy); //if (f < 1) f = 1;
            int widthTh = (int)Math.Round((float)fileWidth / f); int heightTh = (int)Math.Round((float)fileHeight / f);

            // fixes for empty width or height
            if (widthTh == 0)
                widthTh = 1;
            if (heightTh == 0)
                heightTh = 1;

            // Create new image with best quality settings
            Bitmap bp = new Bitmap(widthTh, heightTh);
            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Copy the old image to the new and resized
            Rectangle rect = new Rectangle(0, 0, widthTh, heightTh);
            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            // Copy metadata
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo codec = null;
            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType.Equals("image/jpeg"))
                    codec = codecs[i];
            }

            // Set compresion ratio to 90%
            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(Encoder.Quality, 90L);

            // Save the new image
            bp.Save(thumbnailFileName, codec, ep);
            bp.Dispose();
            g.Dispose();
            
        }
		
	
		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			if (this.Text != null && this.Text != "") 
			{
				string ext = _text.Substring(_text.LastIndexOf(".")+1, _text.Length-_text.LastIndexOf(".")-1);
                string fileNameThumb = umbraco.GlobalSettings.Path + "/.." + _text.Replace("." + ext, "_thumb.jpg");
				bool hasThumb = false;
				try 
				{
					hasThumb = File.Exists(System.Web.HttpContext.Current.Server.MapPath(fileNameThumb));
				} 
				catch {}
				if (hasThumb) 
				{
                    output.WriteLine("<a href=\"" + umbraco.GlobalSettings.Path + "/.." + _text + "\" target=\"_blank\"><img src=\"" + fileNameThumb + "\" border=\"0\"/></a><br/>");
				} 
				else
                    output.WriteLine("<a href=\"" + umbraco.GlobalSettings.Path + "/.." + this.Text + "\" target=\"_blank\">" + this.Text + "</a><br/>");
				output.WriteLine("<input type=\"checkbox\" id=\"" + this.ClientID + "clear\" name=\"" + this.ClientID + "clear\" value=\"1\"/> <label for=\"" + this.ClientID + "clear\">" + ui.Text("uploadClear") + "</label><br/>");
			}
			base.Render(output);
		}
	}
}
