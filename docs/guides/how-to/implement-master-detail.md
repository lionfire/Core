# How-To: Implement Master-Detail Pattern

## Problem

You need to implement a master-detail view where selecting an item from a list displays its details in a side panel or separate view.

## Solution

Use LionFire's observable collections with ReactiveUI selection tracking and Blazor component composition for synchronized master-detail views.

---

## Pattern 1: Side-by-Side Master-Detail

**Use case**: List on left, details on right.

### Step 1: Create the Page ViewModel

```csharp
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using LionFire.Data.Async.Mvvm;

public class ProductsPage : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public ProductsPage(IProductRepository repository)
    {
        this.repository = repository;
        products = new SourceCache<Product, string>(p => p.Id);

        // Transform to ViewModels
        products.Connect()
            .Transform(p => new ProductVM(p, repository))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var boundProducts)
            .Subscribe();

        Products = boundProducts;

        // Load command
        LoadCommand = ReactiveCommand.CreateFromTask(LoadProductsAsync);

        // Create command
        CreateCommand = ReactiveCommand.Create(CreateProduct);

        // Delete command - enabled when item selected
        var canDelete = this.WhenAnyValue(x => x.SelectedProduct)
            .Select(product => product != null);
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteSelectedAsync, canDelete);

        // Save command - enabled when item selected and has changes
        var canSave = this.WhenAnyValue(x => x.SelectedProduct)
            .Select(product => product != null);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveSelectedAsync, canSave);
    }

    private readonly IProductRepository repository;

    public ReadOnlyObservableCollection<ProductVM> Products { get; }

    [Reactive] private ProductVM? _selectedProduct;
    public ProductVM? SelectedProduct
    {
        get => _selectedProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
    }

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    private async Task LoadProductsAsync()
    {
        var allProducts = await repository.GetAllAsync();
        products.AddOrUpdate(allProducts);

        // Select first item
        SelectedProduct = Products.FirstOrDefault();
    }

    private void CreateProduct()
    {
        var newProduct = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "New Product",
            Price = 0.0m,
            Category = "Uncategorized"
        };

        products.AddOrUpdate(newProduct);
        SelectedProduct = Products.First(p => p.Model.Id == newProduct.Id);
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedProduct == null) return;

        await repository.DeleteAsync(SelectedProduct.Model.Id);
        products.Remove(SelectedProduct.Model);

        // Select next item
        SelectedProduct = Products.FirstOrDefault();
    }

    private async Task SaveSelectedAsync()
    {
        if (SelectedProduct == null) return;

        await repository.SaveAsync(SelectedProduct.Model);
        Console.WriteLine($"✅ Saved: {SelectedProduct.Name}");
    }
}

public class ProductVM : ViewModel<Product>
{
    public ProductVM(Product product, IProductRepository repository) : base(product)
    {
        this.repository = repository;
    }

    private readonly IProductRepository repository;

    public string Name
    {
        get => Model.Name;
        set => Model.Name = value;
    }

    public decimal Price
    {
        get => Model.Price;
        set => Model.Price = value;
    }

    public string Category
    {
        get => Model.Category;
        set => Model.Category = value;
    }

    public string Description
    {
        get => Model.Description;
        set => Model.Description = value;
    }
}
```

### Step 2: Blazor Component

```razor
@page "/products"
@inject IProductRepository Repository
@implements IDisposable

<MudGrid>
    <!-- Master: List on left -->
    <MudItem xs="12" md="4">
        <MudCard>
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6">Products</MudText>
                </CardHeaderContent>
                <MudCardActions>
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Add"
                        OnClick="@(() => page.CreateCommand.Execute().Subscribe())" />
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Refresh"
                        OnClick="@(() => page.LoadCommand.Execute().Subscribe())" />
                </MudCardActions>
            </MudCardHeader>
            <MudCardContent>
                <MudList Clickable="true" @bind-SelectedValue="selectedValue">
                    @foreach (var product in page.Products)
                    {
                        <MudListItem Value="@product" OnClick="@(() => page.SelectedProduct = product)">
                            <div class="d-flex justify-space-between">
                                <MudText>@product.Name</MudText>
                                <MudText Typo="Typo.caption">$@product.Price</MudText>
                            </div>
                        </MudListItem>
                    }
                </MudList>
            </MudCardContent>
        </MudCard>
    </MudItem>

    <!-- Detail: Form on right -->
    <MudItem xs="12" md="8">
        @if (page.SelectedProduct != null)
        {
            <MudCard>
                <MudCardHeader>
                    <CardHeaderContent>
                        <MudText Typo="Typo.h6">Product Details</MudText>
                    </CardHeaderContent>
                    <MudCardActions>
                        <MudButton
                            Color="Color.Primary"
                            StartIcon="@Icons.Material.Filled.Save"
                            OnClick="@(() => page.SaveCommand.Execute().Subscribe())">
                            Save
                        </MudButton>
                        <MudButton
                            Color="Color.Error"
                            StartIcon="@Icons.Material.Filled.Delete"
                            OnClick="@(() => page.DeleteCommand.Execute().Subscribe())">
                            Delete
                        </MudButton>
                    </MudCardActions>
                </MudCardHeader>
                <MudCardContent>
                    <MudTextField
                        @bind-Value="page.SelectedProduct.Name"
                        Label="Name"
                        Required="true" />

                    <MudNumericField
                        @bind-Value="page.SelectedProduct.Price"
                        Label="Price"
                        Min="0"
                        Format="F2" />

                    <MudTextField
                        @bind-Value="page.SelectedProduct.Category"
                        Label="Category" />

                    <MudTextField
                        @bind-Value="page.SelectedProduct.Description"
                        Label="Description"
                        Lines="5" />
                </MudCardContent>
            </MudCard>
        }
        else
        {
            <MudCard>
                <MudCardContent>
                    <MudText Typo="Typo.caption" Class="text-center">
                        Select a product to view details
                    </MudText>
                </MudCardContent>
            </MudCard>
        }
    </MudItem>
</MudGrid>

@code {
    private ProductsPage page = null!;
    private IDisposable? subscription;
    private object? selectedValue;

    protected override void OnInitialized()
    {
        page = new ProductsPage(Repository);

        // Reactive updates
        subscription = page.WhenAnyValue(p => p.SelectedProduct)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.LoadCommand.Execute();
        }
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Pattern 2: Tabs for Master-Detail

**Use case**: Use tabs to switch between list and detail views.

```razor
@page "/products-tabs"
@inject IProductRepository Repository
@implements IDisposable

<MudCard>
    <MudTabs @bind-ActivePanelIndex="activeTab">
        <!-- Master Tab -->
        <MudTabPanel Text="All Products">
            <MudCardHeader>
                <MudCardActions>
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Add"
                        OnClick="@CreateAndShowDetail" />
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Refresh"
                        OnClick="@(() => page.LoadCommand.Execute().Subscribe())" />
                </MudCardActions>
            </MudCardHeader>
            <MudCardContent>
                <MudTable Items="@page.Products" Hover="true" OnRowClick="ShowDetail">
                    <HeaderContent>
                        <MudTh>Name</MudTh>
                        <MudTh>Price</MudTh>
                        <MudTh>Category</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.Name</MudTd>
                        <MudTd>$@context.Price.ToString("F2")</MudTd>
                        <MudTd>@context.Category</MudTd>
                    </RowTemplate>
                </MudTable>
            </MudCardContent>
        </MudTabPanel>

        <!-- Detail Tab -->
        <MudTabPanel Text="Details" Disabled="@(page.SelectedProduct == null)">
            @if (page.SelectedProduct != null)
            {
                <MudCardContent>
                    <MudTextField
                        @bind-Value="page.SelectedProduct.Name"
                        Label="Name" />

                    <MudNumericField
                        @bind-Value="page.SelectedProduct.Price"
                        Label="Price" />

                    <MudTextField
                        @bind-Value="page.SelectedProduct.Category"
                        Label="Category" />

                    <MudTextField
                        @bind-Value="page.SelectedProduct.Description"
                        Label="Description"
                        Lines="5" />
                </MudCardContent>
                <MudCardActions>
                    <MudButton
                        Color="Color.Primary"
                        OnClick="@SaveAndReturnToList">
                        Save & Return
                    </MudButton>
                    <MudButton OnClick="@(() => activeTab = 0)">
                        Cancel
                    </MudButton>
                    <MudButton
                        Color="Color.Error"
                        OnClick="@DeleteAndReturnToList">
                        Delete
                    </MudButton>
                </MudCardActions>
            }
        </MudTabPanel>
    </MudTabs>
</MudCard>

@code {
    private ProductsPage page = null!;
    private IDisposable? subscription;
    private int activeTab = 0;

    protected override void OnInitialized()
    {
        page = new ProductsPage(Repository);

        subscription = page.WhenAnyValue(p => p.SelectedProduct)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.LoadCommand.Execute();
        }
    }

    private void ShowDetail(TableRowClickEventArgs<ProductVM> args)
    {
        page.SelectedProduct = args.Item;
        activeTab = 1; // Switch to detail tab
    }

    private void CreateAndShowDetail()
    {
        page.CreateCommand.Execute().Subscribe();
        activeTab = 1;
    }

    private async Task SaveAndReturnToList()
    {
        await page.SaveCommand.Execute();
        activeTab = 0;
    }

    private async Task DeleteAndReturnToList()
    {
        await page.DeleteCommand.Execute();
        activeTab = 0;
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Pattern 3: Modal Detail View

**Use case**: Open detail in a modal dialog.

```razor
@page "/products-modal"
@inject IProductRepository Repository
@inject IDialogService DialogService
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Products</MudText>
        </CardHeaderContent>
        <MudCardActions>
            <MudButton
                Color="Color.Primary"
                StartIcon="@Icons.Material.Filled.Add"
                OnClick="OpenCreateDialog">
                New Product
            </MudButton>
        </MudCardActions>
    </MudCardHeader>
    <MudCardContent>
        <MudTable Items="@page.Products" Hover="true" OnRowClick="OpenDetailDialog">
            <HeaderContent>
                <MudTh>Name</MudTh>
                <MudTh>Price</MudTh>
                <MudTh>Category</MudTh>
                <MudTh>Actions</MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd>@context.Name</MudTd>
                <MudTd>$@context.Price.ToString("F2")</MudTd>
                <MudTd>@context.Category</MudTd>
                <MudTd>
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Edit"
                        Size="Size.Small"
                        OnClick="@(() => OpenDetailDialog(context))" />
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Delete"
                        Size="Size.Small"
                        Color="Color.Error"
                        OnClick="@(() => DeleteProduct(context))" />
                </MudTd>
            </RowTemplate>
        </MudTable>
    </MudCardContent>
</MudCard>

@code {
    private ProductsPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new ProductsPage(Repository);

        subscription = page.Products.ToObservableChangeSet()
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.LoadCommand.Execute();
        }
    }

    private async Task OpenDetailDialog(TableRowClickEventArgs<ProductVM> args)
    {
        await OpenDetailDialog(args.Item);
    }

    private async Task OpenDetailDialog(ProductVM product)
    {
        var parameters = new DialogParameters { ["Product"] = product };
        var dialog = await DialogService.ShowAsync<ProductDetailDialog>("Product Details", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            // Refresh or handle save
            await page.SaveCommand.Execute();
        }
    }

    private async Task OpenCreateDialog()
    {
        page.CreateCommand.Execute().Subscribe();
        if (page.SelectedProduct != null)
        {
            await OpenDetailDialog(page.SelectedProduct);
        }
    }

    private async Task DeleteProduct(ProductVM product)
    {
        page.SelectedProduct = product;
        await page.DeleteCommand.Execute();
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

### Product Detail Dialog Component

```razor
@* ProductDetailDialog.razor *@
<MudDialog>
    <DialogContent>
        <MudTextField
            @bind-Value="Product.Name"
            Label="Name"
            Required="true" />

        <MudNumericField
            @bind-Value="Product.Price"
            Label="Price"
            Min="0"
            Format="F2" />

        <MudTextField
            @bind-Value="Product.Category"
            Label="Category" />

        <MudTextField
            @bind-Value="Product.Description"
            Label="Description"
            Lines="5" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Primary" OnClick="Save">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public ProductVM Product { get; set; } = null!;

    private void Save() => MudDialog.Close(DialogResult.Ok(true));
    private void Cancel() => MudDialog.Cancel();
}
```

---

## Pattern 4: File-Based Master-Detail

**Use case**: Documents stored in filesystem with ObservableReaderVM.

```csharp
using LionFire.Reactive.Persistence;
using LionFire.Data.Async.Mvvm;

public class DocumentsPage : ReactiveObject
{
    public DocumentsPage(string documentsPath)
    {
        // Observable file reader
        var reader = ObservableFsDocuments.Create<DocumentData>(
            dir: documentsPath,
            deserialize: bytes => JsonSerializer.Deserialize<DocumentData>(bytes)!
        ).AsObservableCache();

        // Wrap in ObservableReaderVM
        DocumentsReader = new ObservableReaderVM<string, DocumentData>(reader);

        // Transform to ViewModels
        reader.Connect()
            .Transform(data => new DocumentVM(data, documentsPath))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var documents)
            .Subscribe();

        Documents = documents;

        // Selection
        this.WhenAnyValue(x => x.SelectedDocumentKey)
            .Subscribe(key =>
            {
                SelectedDocument = Documents.FirstOrDefault(d => d.Key == key);
            });

        // Save command
        SaveCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                if (SelectedDocument == null) return;
                await SelectedDocument.SaveAsync();
            },
            this.WhenAnyValue(x => x.SelectedDocument)
                .Select(doc => doc != null)
        );
    }

    public ObservableReaderVM<string, DocumentData> DocumentsReader { get; }
    public ReadOnlyObservableCollection<DocumentVM> Documents { get; }

    [Reactive] private string? _selectedDocumentKey;
    public string? SelectedDocumentKey
    {
        get => _selectedDocumentKey;
        set => this.RaiseAndSetIfChanged(ref _selectedDocumentKey, value);
    }

    [Reactive] private DocumentVM? _selectedDocument;
    public DocumentVM? SelectedDocument
    {
        get => _selectedDocument;
        set => this.RaiseAndSetIfChanged(ref _selectedDocument, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
}

public class DocumentVM : ViewModel<DocumentData>
{
    private readonly string documentsPath;

    public DocumentVM(DocumentData data, string documentsPath) : base(data)
    {
        this.documentsPath = documentsPath;
    }

    public string Key => Model.FileName;

    public string Title
    {
        get => Model.Title;
        set => Model.Title = value;
    }

    public string Content
    {
        get => Model.Content;
        set => Model.Content = value;
    }

    public async Task SaveAsync()
    {
        var filePath = Path.Combine(documentsPath, Model.FileName);
        var json = JsonSerializer.Serialize(Model);
        await File.WriteAllTextAsync(filePath, json);
        Console.WriteLine($"✅ Saved: {Model.FileName}");
    }
}
```

### Blazor Component for File-Based Documents

```razor
@page "/documents"
@implements IDisposable

<MudGrid>
    <MudItem xs="12" md="4">
        <MudCard>
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.h6">Documents</MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                @if (page.DocumentsReader.IsLoading)
                {
                    <MudProgressCircular Indeterminate="true" />
                }
                else
                {
                    <MudList Clickable="true">
                        @foreach (var doc in page.Documents)
                        {
                            <MudListItem
                                Value="@doc.Key"
                                OnClick="@(() => page.SelectedDocumentKey = doc.Key)">
                                <MudText>@doc.Title</MudText>
                            </MudListItem>
                        }
                    </MudList>
                }
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" md="8">
        @if (page.SelectedDocument != null)
        {
            <MudCard>
                <MudCardHeader>
                    <CardHeaderContent>
                        <MudText Typo="Typo.h6">Edit Document</MudText>
                    </CardHeaderContent>
                    <MudCardActions>
                        <MudButton
                            Color="Color.Primary"
                            OnClick="@(() => page.SaveCommand.Execute().Subscribe())">
                            Save
                        </MudButton>
                    </MudCardActions>
                </MudCardHeader>
                <MudCardContent>
                    <MudTextField
                        @bind-Value="page.SelectedDocument.Title"
                        Label="Title" />

                    <MudTextField
                        @bind-Value="page.SelectedDocument.Content"
                        Label="Content"
                        Lines="20" />
                </MudCardContent>
            </MudCard>
        }
        else
        {
            <MudText Class="text-center mt-8">Select a document to edit</MudText>
        }
    </MudItem>
</MudGrid>

@code {
    [Parameter] public string DocumentsPath { get; set; } = "./documents";

    private DocumentsPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new DocumentsPage(DocumentsPath);

        subscription = page.WhenAnyValue(
                p => p.SelectedDocument,
                p => p.DocumentsReader.IsLoading
            )
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Best Practices

### 1. Always Track Selection

```csharp
// ✅ Good - Reactive selection tracking
[Reactive] private ProductVM? _selectedProduct;
public ProductVM? SelectedProduct
{
    get => _selectedProduct;
    set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
}

// ❌ Avoid - No change notification
public ProductVM? SelectedProduct { get; set; }
```

### 2. Handle Empty Selection

```razor
<!-- ✅ Good - Show empty state -->
@if (page.SelectedProduct != null)
{
    <ProductDetails Product="@page.SelectedProduct" />
}
else
{
    <MudText>Select a product to view details</MudText>
}

<!-- ❌ Avoid - Null reference crash -->
<ProductDetails Product="@page.SelectedProduct" />
```

### 3. Commands Based on Selection

```csharp
// ✅ Good - Delete enabled only when selected
var canDelete = this.WhenAnyValue(x => x.SelectedProduct)
    .Select(product => product != null);
DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync, canDelete);

// ❌ Avoid - No selection check
DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
```

### 4. Update Selection After Delete

```csharp
// ✅ Good - Select next item after delete
private async Task DeleteSelectedAsync()
{
    if (SelectedProduct == null) return;

    await repository.DeleteAsync(SelectedProduct.Model.Id);
    products.Remove(SelectedProduct.Model);

    // Select next item
    SelectedProduct = Products.FirstOrDefault();
}

// ❌ Avoid - Leave selection dangling
private async Task DeleteSelectedAsync()
{
    await repository.DeleteAsync(SelectedProduct!.Model.Id);
    products.Remove(SelectedProduct.Model);
    // SelectedProduct still points to deleted item!
}
```

---

## Summary

**Master-Detail Patterns:**

1. **Side-by-Side** - List on left, details on right
2. **Tabs** - Switch between list and detail views
3. **Modal** - Open details in dialog
4. **File-Based** - Documents with ObservableReaderVM

**Key Points:**
- Use reactive selection tracking
- Always handle empty selection
- Enable/disable commands based on selection
- Update selection after delete
- Dispose subscriptions properly

**Related Guides:**
- [Create Custom ViewModels](create-custom-viewmodel.md)
- [Bind Async Data to UI](bind-async-data-to-ui.md)
- [Reactive Collections](../getting-started/04-reactive-collections.md)
- [MVVM Domain Docs](../../mvvm/README.md)
