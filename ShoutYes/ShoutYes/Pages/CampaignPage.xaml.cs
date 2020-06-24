using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoutYes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CampaignPage : ContentPage
    {
        private const string ApiKey = "your api key";
        private const string ListId = "bd7d8f1b64";
        private const int CampaignId = 1;
        private const int TemplateId = 1; 

        IMailChimpManager mailChimpManager = new MailChimpManager(ApiKey);
        Member tempMember = null;

        private MailChimpManager _mailChimpManager = new MailChimpManager(ApiKey);
        private Setting _campaignSettings = new Setting
        {
            ReplyTo = "milad@bebusiness.nz",
            FromName = "Milad",
            Title = "Your campaign title",
            SubjectLine = "The email subject",
        };

        // `html` contains the content of your email using html notation
        public void CreateAndSendCampaign(string html)
        {
            var campaign = _mailChimpManager.Campaigns.AddAsync(new Campaign
            {
                Settings = _campaignSettings,
                Recipients = new Recipient { ListId = ListId },
                Type = CampaignType.Regular
            }).Result;
            var timeStr = DateTime.Now.ToString();
            var content = _mailChimpManager.Content.AddOrUpdateAsync(
             campaign.Id,
             new ContentRequest()
             {
                 Template = new ContentTemplate
                 {
                     Id = TemplateId,
                     Sections = new Dictionary<string, object>()
                {
       { "body_content", html },
       { "preheader_leftcol_content", $"<p>{timeStr}</p>" }
                }
                 }
             }).Result;
            _mailChimpManager.Campaigns.SendAsync(campaign.Id).Wait();
        }
        public List<Template> GetAllTemplates()
          => _mailChimpManager.Templates.GetAllAsync().Result.ToList();
        public List<List> GetAllMailingLists()
          => _mailChimpManager.Lists.GetAllAsync().Result.ToList();
        public Content GetTemplateDefaultContent(string templateId)
          => (Content)_mailChimpManager.Templates.GetDefaultContentAsync(templateId).Result;

        public CampaignPage()
        {
            InitializeComponent();
            To_List();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var mailChimpListCollection = await this.mailChimpManager.Lists.GetAllAsync().ConfigureAwait(false);
            Debug.WriteLine("****************************");
            foreach (var elem in mailChimpListCollection)
                Debug.WriteLine(elem.Id);
        }

        private async void To_List()
        {
            var listId = "bd7d8f1b64";
            var toListMailChimp = await this.mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);

            toList.ItemsSource = null;

            await Device.InvokeOnMainThreadAsync(() => {
                toList.ItemsSource = toListMailChimp.Select(elem => elem.EmailAddress);
            });
        }

        private async void To_Add_Clicked(object sender, EventArgs e)
        {
            var listId = "bd7d8f1b64";
            // Use the Status property if updating an existing member
            if (emailEntry.Text != "" && fnameEntry.Text != "" && lnameEntry.Text != "")
            {
                var member = new Member { EmailAddress = emailEntry.Text, StatusIfNew = Status.Subscribed };

                member.MergeFields.Add("FNAME", fnameEntry.Text);
                member.MergeFields.Add("LNAME", lnameEntry.Text);
                await this.mailChimpManager.Members.AddOrUpdateAsync(listId, member);

                toList.ItemsSource = null;

                var toListMailChimp = await this.mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);
                await Device.InvokeOnMainThreadAsync(() => {
                    toList.ItemsSource = toListMailChimp.Select(elem => elem.EmailAddress);
                });
            }
            else
            {
                //exception
            }
        }

        private async void Update_List(object sender, EventArgs e)
        {
            var listId = "bd7d8f1b64";
            var members = await this.mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);
            var member = members.First(x => x.EmailAddress == "abc@def.com");

            // Update the user
            member.MergeFields.Add("FNAME", "New first name");
            member.MergeFields.Add("LNAME", "New last name");
            await this.mailChimpManager.Members.AddOrUpdateAsync(listId, member);
        }

        private void Send_Email_Clicked(object sender, EventArgs e)
        {
            /*
            var tt = mailChimpManager.Content.GetAsync("1");
            var bb = mailChimpManager.Templates.GetAllAsync();
            

            Debug.WriteLine("**********************************");
            //foreach(var t in tt)
            Debug.WriteLine(bb.Id);
            Debug.WriteLine("**********************************");
            */
            CreateAndSendCampaign("HELLLLLOOOO");
        }

            /*
             * <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>
                    <Button Text="Add Receipant" Grid.Row="0" Grid.Column="0"
                        Clicked="To_Add_Clicked" 
                        VerticalOptions="EndAndExpand"
                        HorizontalOptions="Fill"/>
                    <Entry Placeholder="Email" Grid.Row="0" Grid.Column="1"
                       VerticalOptions="EndAndExpand"
                       HorizontalOptions="Start" />
                    <Entry Placeholder="First Name" Grid.Row="0" Grid.Column="2"
                       VerticalOptions="EndAndExpand"
                       HorizontalOptions="Start" />
                    <Entry Placeholder="Last Name" Grid.Row="0" Grid.Column="3"
                       VerticalOptions="EndAndExpand"
                       HorizontalOptions="Start" />
                </Grid>
                */
        }
}
 