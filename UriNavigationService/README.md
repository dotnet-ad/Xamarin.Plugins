# UriNavigationService

## Usage

### Platform

```csharp

// Page declaration

[NavigationContext(typeof(CustomerPage)]
public partial class CustomerPage : Page
{
	public override void OnNavigatingTo(object sender, NavigaionArgs args)
	{
		base.OnNavigatingTo(sender,args);
		
		var service = CrossUriNavigationService.Current;
		this.DataContext = service.GetPageContext(args.Param);
	}	
}

// in App.xaml constructor

var service = CrossUriNavigationService.Current;
service.RegisterPage<CustomerPage>()
```

### Portable

```csharp
var service = CrossUriNavigationService.Current;
service.Navigate("/customer/5?isEditable=true");
// ...
service.GoBack();
```

## Context

```csharp
[NavigationUri("/customers/{id}")]
public class CustomerViewModel
{
	[NavigationSegment("id",NavigationRequirement.Optional)]
	public int Identifier { get; set; }
	
	[NavigationQuery("isEditable",NavigationRequirement.Optional)]
	public bool IsEditable { get; set; }
}

```

## View (Windows)

```csharp
[NavigationContext(typeof(CustomerViewModel))]
public partial class CustomerPage
{
	
}

```