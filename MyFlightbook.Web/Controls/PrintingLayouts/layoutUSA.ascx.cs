﻿using MyFlightbook;
using MyFlightbook.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

/******************************************************
 * 
 * Copyright (c) 2016-2020 MyFlightbook LLC
 * Contact myflightbook-at-gmail.com for more information
 *
*******************************************************/

public partial class Controls_PrintingLayouts_layoutUSA : System.Web.UI.UserControl, IPrintingTemplate
{
    public MyFlightbook.Profile CurrentUser { get; set; }

    protected bool ShowFooter { get; set; }

    protected Collection<OptionalColumn> OptionalColumns { get; private set; }

    protected string OtherCatClassValue(LogbookEntryDisplay led)
    {
        return (led != null && led.EffectiveCatClass != (int)CategoryClass.CatClassID.ASEL && led.EffectiveCatClass != (int)CategoryClass.CatClassID.AMEL && OptionalColumn.ShowOtherCatClass(OptionalColumns, (CategoryClass.CatClassID)led.EffectiveCatClass)) ? 
            String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}: {1}", led.CategoryClassNoType, led.TotalFlightTime.FormatDecimal(CurrentUser.UsesHHMM)) : 
            string.Empty;
    }

    #region IPrintingTemplate
    public void BindPages(IEnumerable<LogbookPrintedPage> lst, Profile user, PrintingOptions options, bool showFooter = true)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));
        ShowFooter = showFooter;
        CurrentUser = user;
        OptionalColumns = options.OptionalColumns;

        rptPages.DataSource = lst;
        rptPages.DataBind();
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e) { CurrentUser = MyFlightbook.Profile.GetUser(Page.User.Identity.Name); }

    protected void rptPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        LogbookPrintedPage lep = (LogbookPrintedPage)e.Item.DataItem;

        Repeater rpt = (Repeater)e.Item.FindControl("rptFlight");
        rpt.DataSource = lep.Flights;
        rpt.DataBind();
        
        rpt = (Repeater)e.Item.FindControl("rptSubtotalCollections");
        rpt.DataSource = lep.Subtotals;
        rpt.DataBind();
    }

    protected void rptSubtotalCollections_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        LogbookPrintedPageSubtotalsCollection sc = (LogbookPrintedPageSubtotalsCollection)e.Item.DataItem;
        Repeater rpt = (Repeater)e.Item.FindControl("rptSubtotals");
        rpt.DataSource = sc.Subtotals;
        rpt.DataBind();
    }

    protected void rptFlight_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));
        Controls_mfbSignature sig = (Controls_mfbSignature)e.Item.FindControl("mfbSignature");
        sig.Flight = (LogbookEntryDisplay)e.Item.DataItem;
    }
}