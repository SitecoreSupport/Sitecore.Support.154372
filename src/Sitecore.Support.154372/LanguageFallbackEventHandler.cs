using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Links;
using Sitecore.Publishing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support
{
  public class LanguageFallbackEventHandler
  {
    protected void OnItemSaved(object sender, EventArgs args)
    {
      if (args != null)
      {
        Item item = Event.ExtractParameter(args, 0) as Item;
        ItemChanges itemChanges = Event.ExtractParameter(args, 1) as ItemChanges;
        Assert.IsNotNull(item, "No item in parameters");
        Assert.IsNotNull(itemChanges, "No itemChanges in parameters");
        if (itemChanges.HasFieldsChanged && itemChanges.FieldChanges.Contains(FieldIDs.EnableItemFallback) 
          && itemChanges.IsFieldModified(FieldIDs.EnableItemFallback) && string.IsNullOrEmpty(itemChanges.GetFieldValue(FieldIDs.EnableItemFallback)))
        {
          foreach (var index in ContentSearchManager.Indexes.Where(i => i.EnableItemLanguageFallback))
          {
            foreach (var crawler in index.Crawlers)
            {
              var crawlerObject = crawler as SitecoreItemCrawler;
              if (crawlerObject != null && string.Equals(crawlerObject.Database, item.Database.Name, StringComparison.InvariantCultureIgnoreCase))
              {
                var indexableId = new SitecoreItemId(item.ID);
                IndexCustodian.DeleteItem(index, indexableId);
                break;
              }
            }
          }
        }
      }
    }

  }
}