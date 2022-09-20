<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM501000.aspx.cs" Inherits="Page_LM501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="UCG_Customization.LMTeleTransProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule6" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule8" StartGroup="True" GroupCaption="Filter" ></px:PXLayoutRule>
			<px:PXPanel runat="server" ID="CstPanel11">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask14" DataField="BranchID" ></px:PXSegmentMask>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector23" DataField="PayTypeID" ></px:PXSelector>
				<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector26" DataField="CashAccountID" ></px:PXSelector>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule13" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector24" DataField="BAccountID" ></px:PXSelector>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit25" DataField="DocDate" ></px:PXDateTimeEdit>
				<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox18" DataField="IsTTGenerated" ></px:PXCheckBox></px:PXPanel>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="Summary" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit22" DataField="PayDate" ></px:PXDateTimeEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit20" DataField="CurySelTotal" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit21" DataField="SelCount" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" TextAlign="Center" AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AcctName" Width="280" />
				<px:PXGridColumn DataField="DocDesc" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryOrigDocAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>
