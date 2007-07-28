using System;
using System.Collections;
using CookComputing.MetaWeblog;
using CookComputing.XmlRpc;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.web;
using umbraco.presentation.channels.businesslogic;

namespace umbraco.presentation.channels
{
    /// <summary>
    /// Summary description for Test.
    /// </summary>
    [XmlRpcService(
        Name = "umbraco metablog test",
        Description = "For editing umbraco data from external clients",
        AutoDocumentation = true)]
    public class api : UmbracoMetaWeblogAPI, IRemixWeblogApi
    {
        public api()
        {
        }


        [XmlRpcMethod("metaWeblog.newMediaObject",
            Description = "Makes a new file to a designated blog using the "
                          + "metaWeblog API. Returns url as a string of a struct.")]
        public UrlData newMediaObject(
            string blogid,
            string username,
            string password,
            FileData file)
        {
            return newMediaObjectLogic(blogid, username, password, file);
        }

        #region IRemixWeblogApi Members

        public wpPageSummary[] getPageList(string blogid, string username, string password)
        {
            if (User.validateCredentials(username, password, false))
            {
                ArrayList blogPosts = new ArrayList();
                ArrayList blogPostsObjects = new ArrayList();

                User u = new User(username);
                Channel userChannel = new Channel(u.Id);


                Document rootDoc;
                if (userChannel.StartNode > 0)
                    rootDoc = new Document(userChannel.StartNode);
                else
                    rootDoc = new Document(u.StartNodeId);


                foreach (Document d in rootDoc.Children)
                {
                    int count = 0;
                    blogPosts.AddRange(
                        findBlogPosts(userChannel, d, u.Name, ref count, 999, userChannel.FullTree));
                }

                blogPosts.Sort(new DocumentSortOrderComparer());

                foreach (Object o in blogPosts)
                {
                    Document d = (Document) o;
                    wpPageSummary p = new wpPageSummary();
                    p.dateCreated = d.CreateDateTime;
                    p.page_title = d.Text;
                    p.page_id = d.Id;
                    p.page_parent_id = d.Parent.Id;

                    blogPostsObjects.Add(p);
                }


                return (wpPageSummary[]) blogPostsObjects.ToArray(typeof (wpPageSummary));
            }
            else
            {
                return null;
            }
        }

        public wpPage[] getPages(string blogid, string username, string password, int numberOfItems)
        {
            if (User.validateCredentials(username, password, false))
            {
                ArrayList blogPosts = new ArrayList();
                ArrayList blogPostsObjects = new ArrayList();

                User u = new User(username);
                Channel userChannel = new Channel(u.Id);


                Document rootDoc;
                if (userChannel.StartNode > 0)
                    rootDoc = new Document(userChannel.StartNode);
                else
                    rootDoc = new Document(u.StartNodeId);


                foreach (Document d in rootDoc.Children)
                {
                    int count = 0;
                    blogPosts.AddRange(
                        findBlogPosts(userChannel, d, u.Name, ref count, numberOfItems, userChannel.FullTree));
                }

                blogPosts.Sort(new DocumentSortOrderComparer());

                foreach (Object o in blogPosts)
                {
                    Document d = (Document) o;
                    wpPage p = new wpPage();
                    p.dateCreated = d.CreateDateTime;
                    p.title = d.Text;
                    p.page_id = d.Id;
                    p.wp_page_parent_id = d.Parent.Id;
                    p.wp_page_parent_title = d.Parent.Text;
                    p.permalink = library.NiceUrl(d.Id);
                    p.description = d.getProperty(userChannel.FieldDescriptionAlias).Value.ToString();
                    p.link = library.NiceUrl(d.Id);

                    if (userChannel.FieldCategoriesAlias != null && userChannel.FieldCategoriesAlias != "" &&
                        d.getProperty(userChannel.FieldCategoriesAlias) != null &&
                        ((string) d.getProperty(userChannel.FieldCategoriesAlias).Value) != "")
                    {
                        String categories = d.getProperty(userChannel.FieldCategoriesAlias).Value.ToString();
                        char[] splitter = {','};
                        String[] categoryIds = categories.Split(splitter);
                        p.categories = categoryIds;
                    }


                    blogPostsObjects.Add(p);
                }


                return (wpPage[]) blogPostsObjects.ToArray(typeof (wpPage));
            }
            else
            {
                return null;
            }
        }

        public string newCategory(
            string blogid,
            string username,
            string password,
            wpCategory category)
        {
            if (User.validateCredentials(username, password, false))
            {
                Channel userChannel = new Channel(username);
                if (userChannel.FieldCategoriesAlias != null && userChannel.FieldCategoriesAlias != "")
                {
                    // Find the propertytype via the document type
                    ContentType blogPostType = ContentType.GetByAlias(userChannel.DocumentTypeAlias);
                    PropertyType categoryType = blogPostType.getPropertyType(userChannel.FieldCategoriesAlias);

                    PreValue pv = new PreValue();
                    pv.DataTypeId = categoryType.DataTypeDefinition.Id;
                    pv.Value = category.name;
                    pv.Save();
                }
            }
            return "";
        }

        #endregion
    }
}