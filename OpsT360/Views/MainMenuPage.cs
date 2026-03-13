namespace OpsT360.Views;

public class MainMenuPage : TabbedPage
{
    public MainMenuPage(TransactionsPage transactionsPage, SealInspectionPage sealInspectionPage)
    {
        Title = "Ops T360";

        Children.Add(new NavigationPage(transactionsPage)
        {
            Title = "Transactions"
        });

        Children.Add(new NavigationPage(sealInspectionPage)
        {
            Title = "Seals"
        });
    }
}
