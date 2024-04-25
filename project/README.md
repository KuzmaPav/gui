# WPF Async Projekt

# Stahovač Obrázků

## Zadání:
- Vytvořit aplikaci, která při zadání url adresy obrázku, začne její stahování a uložení do souboru.

## Ukázka:
![Vzhled aplikace](imgs/ukazka_aplikace1.jpg)
- Vzhled aplikace
---
![Zadání url adresy obrázku](imgs/ukazka_aplikace2.jpg)
- Zadání url adresy obrázku
---
![Vytvoření elementu reprezentující stahování](imgs/ukazka_aplikace3.jpg)
- Vytvoření elementu reprezentující stahování
---
![Zakončení stahování a odebrání elementu](imgs/ukazka_aplikace4.jpg)
- Zakončení stahování a odebrání elementu
---

## Rozvržení funkcionality
### Třídy:
- DownloadTask (Klient) - asynchronní stahování z internetu
- DownloadElement - Zaobalující třída s vizualizací
- MainWindow - ...
---

![Propojení tříd](imgs/architektura_trid.jpg)
- Propojení tříd
---

## Tvorba aplikace
1. **XAML design**

- Nejprve si uděláme nějaký jednoduchý XAML design. Může být libovolný, ale musí splňovat:
  - 1 **`StackPanel`** pro generované **`DownloadElementy`**
  - 1 **`TextBox`** pro zadání url adresy
  - 2 **`Buttony`** pro 'Download' a 'Finalize'

- Moje řešení má tento design:
  - 2 **`Gridy`**
  - 2 **`StackPanely`**
  - 3 **`Buttony`**
  - 1 **`TextBox`**
  - 1 **`Label`**

- Řešení:

```xml
<Grid Margin="10,10,10,10">
	<Grid.RowDefinitions>
		<RowDefinition Height="auto"/>
		<RowDefinition Height="*"/>
	</Grid.RowDefinitions>

	<Label Grid.Row="0" Content="Image Downloader" HorizontalContentAlignment="Center" FontSize="32"/>

	<Grid Grid.Row="1">
		<Grid.ColumnDefinitions>
			<ColumnDefinition Width="*"/>
			<ColumnDefinition Width="auto"/>
		</Grid.ColumnDefinitions>
		
		<StackPanel x:Name="stackPanel_downloadElems" Grid.Column="0" MinWidth="200">

			<!-- List of downloading elements -->

		</StackPanel>

		<StackPanel Grid.Column="1" Width="200" MinWidth="100" MaxWidth="200" Height="250">

			<!-- Controling elements -->
			
			<Label Content="Adresa Obrázku" HorizontalAlignment="Center" FontSize="15"/>
			<TextBox x:Name="textBox_downLink" Margin="5" Height="auto" FontSize="15" Padding="5"/>
			<Button Content="Download" Click="InitiateDownload" Margin="5" Height="30" FontSize="15"/>
			<Button x:Name="ButtonEdit" Content="Edit" IsEnabled="False" Margin="5" Height="30" FontSize="15"/>
			<Button x:Name="ButtonFinalize" Content="Close element" IsEnabled="False" Margin="5" Height="30" FontSize="15"/>

		</StackPanel>
	</Grid>
</Grid>
```
---

2. **Definování tříd**

- V aplikaci použijeme 2 vlastní třídy **`DownloadTask`** a **`DownloadElement`**. Nadefinujeme si jejich zádkladní rozhraní.

  - **`DownloadTask`** bude obsahovat `HttpClienta`, `List` s observery (v tomto případě jenom jedním), url link, ze kterého stahujeme jako `string`, výsledný soubor také jako `string` a `int` o velikosti jednoho chunku.

  - **`DownloadElement`** bude dědit ze třídy `ToggleButton` a bude obsahovat instanci `DownloadTask`, odkaz `MainWindow` na hlavní okno a tyto elementy, se kterými se bude jeětě pracovat: `ProgressBar`, `TextBox`, `Slider` a `Label`.

- Řešení:
  - DownloadTask
```c#
public class ImageDownloadTask
{
	HttpClient downloadClient;

	List<DownloadElement> observers = new List<DownloadElement>();

	public string downLink { get; private set; }

	public string targetFile { get; private set; }

	int chunkSize;

	public string state { get; private set; } = "inprogress";

}
```

  - DownloadElement
```c#
public class DownloadElement : ToggleButton
{
	public ImageDownloadTask downloadTask { get; private set; }

	ProgressBar progressBar;

	TextBox limitTextBox;

	Slider limitSlider;

	Label linkLabel;

	MainWindow parentWindow;

}
```
---

3. **Konstruktory tříd**

- Nyní si vytvoříme konstruktory pro obě třídy.

  - **`DownloadTask`** bude v konstruktoru vytváří instanci `HttpClienta` a nastaví atributy podle převzatých parametrů: 
	+ `string` url ceasta k obrázku
	+ `string` cesta pro výstupní soubor
	+ `int` chunkSize, který je defaultně =10


  - **`DownloadElement`** v konstruktoru se budou nastavovat různé parametry jak svoje (ToggleButton), ale i elementů vně. Tyto elementy budou následovat toto xaml schéma:

```xml
<ToggleButton BorderThickness="2" Height="50" Margin="10">
	<StackPanel Orientation="Horizontal">
		<Label Content="URL download link" MaxWidth="200" Margin="5"/>
		<Label Content="Down limit:" Margin="5,5,0,5"/>
		<TextBox Text="10" Width="40" Height="20" Margin="0,5,0,5" TextChanged="LimitTextBox_TextChanged"/>
		<Label Content="KB/s" Margin="0,5,5,5"/>
		<Slider Minimum="1" Maximum="1000" Value="10" Width="50" Height="20" Margin="5" ValueChanged="LimitSlider_ValueChanged"/>
		<ProgressBar MinWidth="25" Width="100" Minimum="0" Maximum="1" Height="20" Margin="5"/>
	</StackPanel>
</ToggleButton>
``` 
  - a bude přebírat tyto paramatry:
	+ `DownloadTask`
	+ `MainWindow`

- Řešení:
  - DownloadTask
```c#
public ImageDownloadTask(string downLink, string targetFile, int chunkSize = 10)
{
	this.downloadClient = new HttpClient();
	this.chunkSize = chunkSize;
	this.downLink = downLink;
	this.targetFile = targetFile;
}
```

  - DownloadElement
```c#
public DownloadElement(ImageDownloadTask downloadTask, MainWindow parentWindow)
{
    // Toggle element (Download element) properties:
    BorderThickness = new Thickness(2);
    Height = 50;
    Margin = new Thickness(10);

    // Actions:
    Checked += ThisChecked;
    Unchecked += ThisUnchecked;

    //----------------------------------------------------------------------------------------
    this.parentWindow = parentWindow;

    var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

    linkLabel = new Label { Content = downloadTask.downLink, MaxWidth = 200, Margin = new Thickness(5) };
    stackPanel.Children.Add(linkLabel);

    var downLimitLabel = new Label { Content = "Down limit:", Margin = new Thickness(5, 5, 0, 5) };
    stackPanel.Children.Add(downLimitLabel);

    // Textbox for download speed
    limitTextBox = new TextBox { Text = "10", Width = 40, Height = 20, Margin = new Thickness(0, 5, 0, 5) };
    //limitTextBox.TextChanged += LimitTextBox_TextChanged;
    stackPanel.Children.Add(limitTextBox);

    var unitLabel = new Label { Content = "KB/s", Margin = new Thickness(0, 5, 5, 5) };
    stackPanel.Children.Add(unitLabel);

    // Slider for download speed
    limitSlider = new Slider { Minimum = 1, Maximum = 1_000, Value = 10, Width = 50, Height = 20, Margin = new Thickness(5) };
    //limitSlider.ValueChanged += LimitSlider_ValueChanged;
    stackPanel.Children.Add(limitSlider);

    // ProgressBar to show progress of the download
    progressBar = new ProgressBar { MinWidth = 25, Width = 100, Minimum = 0, Maximum = 1, Height = 20, Margin = new Thickness(5) };
    stackPanel.Children.Add(progressBar);

    Content = stackPanel;

}
```
---

4. **Propojení**

- Teď potřebujeme tyto dvě třídy nějak chytře propojit, aby mohli se sebou komunikovat. V třídě **`DownloadTask`** je připravený prostor pro observer **`DownloadElement`** a v **`DownloadElement`** je v konstruktoru připravená instance třídy **`DownloadTask`**. Tak to prostě jednoduše spojme.

- Do třídy **`DownloadTask`** se přidá metoda pro přidávání do listu observerů a v **`DownloadElement`** se zavolá.

- Řešení:
  - DownloadTask
```c#
public void AddObserver(DownloadElement observer) =>
	observers.Add(observer);
```

  - DownloadElement
```c#
this.downloadTask = downloadTask;
downloadTask.AddObserver(this);
```


5. **DownloadTask Download**

- Vytvoříme si metodu třídy DownloadTask, která bude zahrnovat většinu asynchornní funkcionality.
- Metoda bude kontrolovat: 
  + zda spojení s url vyšlo
  + zda url odpovída
  + a zda se jedná o url obrázku

- Tahle aplikace bude řešit i omezení rychlosti stahování jednotlivých obrázků, pro lepší ukázku asynchronosti.

- Obsah metody
  + Pokus o navázání spojení přes `HttpClienta` a kontrola odpovědi.
  + Následně deklarování proměnných, které budou potřeba při stahování.
  + Logické část stahování a psaní do souboru

- Při stahování se bude i počítat jak rychle se chunk stáhnul a jak dlouho počkat do vteřiny pro splnění rychlosti stahování přes následující komplexní vzorec. $1 \text{ s} = downloadTime \text{ s} + (1 - downloadTime \text{ s})$

- Řešení:
```c#
public async Task Download()
{
    // Exceptions checking
    HttpResponseMessage? response;

    try
    {
        response = await downloadClient.GetAsync(downLink, HttpCompletionOption.ResponseHeadersRead);
    }
    catch
    {
        state = "failed";
        return;
    }
    
    if (!response.IsSuccessStatusCode)
    {
        MessageBox.Show($"Invalid image link\nWeb response: {response.StatusCode}");
        state = "failed";
        return;
    }

    if (!response.Content.Headers.ContentType.ToString().Contains("image"))
    {
        MessageBox.Show("Given url is not an image url.");
        state = "failed";
        return;
    }

    // Declaring variables
    long totalImgSize = response.Content.Headers.ContentLength ?? -1;

    bool canReportState = false;

    if (totalImgSize != -1)
        canReportState = true;

    long totalBytesRead = 0;

    var buffer = new byte[this.chunkSize * updateMulitplier];
    int bytesRead;

    var chunkTimeStart = Stopwatch.StartNew();

    // Logic
    try
    {
        using (var contentStream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                chunkTimeStart.Restart();

                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                if (chunkSize != buffer.Length)
                    buffer = new byte[this.chunkSize * updateMulitplier];


                //if (canReportState)
                    //UpdateProgress((double)totalBytesRead / totalImgSize);

                await Task.Delay(int.Max(0, (1000 * updateMulitplier) - (int)chunkTimeStart.ElapsedMilliseconds));
            }

            state = "finished";
        }
    }

    catch
    {
        state = "failed";
    }
        
}
```

6. **Can't touch this**

- Zatím vše co jsme vytvořili není hmatatelné a porto půjdeme do `MainWindow` a něco málo tam přiděláme.
- Vytvoříme proměnné pro `Buttony`: Edit a Finalize a poté dáme `Buttonu Download` její funkcionalitu. `Button` při zmáčknutí zjistí, jestli je v `TextBoxu` nějaký text a pokud není tak nic neudělá. Pokud je tak zjistí kolik je v `StackPanelu` `DownloadElementů` a pokud je jich méně než 5 tak pokračuje.
- Následuje ukládací okénko, kde uživatel zvolí kam se obrázek bude ukládat.
- Vytvoří se instance `DownloadTask` a `DownloadElement` a zahají se stahování přes Task.Run()

- Řešení:
```c#
public partial class MainWindow : Window
{
    // Store references to finalize and edit buttons
    public Button button_FinalizeDownload { get; private set; }
    public Button button_EditImage { get; private set; }

    public MainWindow()
    {
        InitializeComponent();

        // Assign buttons from XAML to properties
        button_FinalizeDownload = ButtonFinalize;
        button_EditImage = ButtonEdit;
    }

    private void InitiateDownload(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_downLink.Text)) return;

        if (stackPanel_downloadElems.Children.Count >= 5)
        {
            MessageBox.Show("You can download only 5 images at once.", "Max downloads", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }


        var saveDialog = new SaveFileDialog();
        saveDialog.Filter = "JPEG Image|*.jpg";

        if (saveDialog.ShowDialog() == true)
        {
            var downloadTask = new ImageDownloadTask(textBox_downLink.Text, saveDialog.FileName);

            // Create a new instance of DownloadElement
            DownloadElement newDownloadElement = new DownloadElement(downloadTask, this);

            Task.Run(() => newDownloadElement.downloadTask.Download());

            // Add child element to StackPanel element
            stackPanel_downloadElems.Children.Add(newDownloadElement);

            // Clear TextBox element
            textBox_downLink.Text = "";
        }

    }
```

7. **Progres**

- Teď se nám vytvořil element a snad se i začlo stahovat, ale nelze to říci, ikdybychom čekali 10 minut při stahování obrázku 100x100.
- V `DownloadTask` si vytvoříme metodu `UpdateProgress`, která zavolá stejnojmennou metodu každého observera (jenom jednoho), která už upraví Value v `progressBaru`.
- Poté můžeme dodat do download metody 'if can report, report'

- Řešení:
  - DownloadTask
```c#
private void UpdateProgress(double value)
{
    foreach (var observer in observers)
        observer.UpdateProgress(value);
}
```
a
```c#
if (chunkSize != buffer.Length)			//old code
	buffer = new byte[this.chunkSize * updateMulitplier];  //old code


if (canReportState)   //new code
	UpdateProgress((double)totalBytesRead / totalImgSize); //new code

await Task.Delay(int.Max(0, (1000 * updateMulitplier) - (int)chunkTimeStart.ElapsedMilliseconds));  //old code
```

  - DownloadElement
```c#
public void UpdateProgress(double value)
{
    Application.Current.Dispatcher.Invoke(() =>
    {
        progressBar.Value = value;
    });
}
```


8. **Limit**

- Teď se nám načíta `ProgressBar`, ale je to nějak pomalý. Nějaký šprímař nastavil zákldní rychlost stahování na 10 KB/s. 
- Nejdříve si propojíme chování `Slideru` a `TextBoxu` v `DownloadElement`. K oboum těmto elmentům is vytvoříme metodu, která se zavolá, když se změní hodnota a upraví hodnotu toho druhého elementu.
- Tyto metody předají hodnotu `TextBoxu` (TextBox > Slider) do metody `ChangeLimiter` v třídě `DownloadTask`, která změní atribut chunkSize.

 - Řešení:
  - DownloadTask
```c#
public void ChangeLimiter(int new_limit) =>
	this.chunkSize = new_limit;
```

  - DownloadElement
```c#
private void LimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
{
	if (int.TryParse(limitTextBox.Text, out int value))
	{
		limitSlider.Value = value;
		downloadTask.ChangeLimiter(value);
	}
}

private void LimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
{
	limitTextBox.Text = Math.Round(limitSlider.Value, 0).ToString();
	downloadTask.ChangeLimiter(int.Parse(limitTextBox.Text));
}
```
a
```c#
// Textbox for download speed
limitTextBox = new TextBox { Text = "10", Width = 40, Height = 20, Margin = new Thickness(0, 5, 0, 5) };  //old code
limitTextBox.TextChanged += LimitTextBox_TextChanged;   //new code
stackPanel.Children.Add(limitTextBox);   //old code

...

// Slider for download speed
limitSlider = new Slider { Minimum = 1, Maximum = 1_000, Value = 10, Width = 50, Height = 20, Margin = new Thickness(5) };   //old code
limitSlider.ValueChanged += LimitSlider_ValueChanged;  //new code
stackPanel.Children.Add(limitSlider);   //old code
```

9. **Too many**

- Žádný kód není dokonalý, ale můžeme se trošku přiblíž k tomu (very, very little). 
- Teď zajistíme, aby se nemohlo checknout více `DownloadElementů` najednou.
- Vložíme metodu `Check` a `Uncheck` do `DownloadElementu` na který už jsme se snažili dostat již dříve.
- A v `MainWindow` vytvoříme metodu `SingleToggle`, která při zavolání projde elementy `Stackpanelu` a uncheckne je.

 - Řešení:
  - DownloadElement
```c#
private void ThisChecked(object sender, RoutedEventArgs e)
{
	parentWindow.SingleToggle(sender);

	if (downloadTask.state == "finished")
	{
		parentWindow.button_EditImage.IsEnabled = true;
		parentWindow.button_EditImage.Click += EditImage;

		parentWindow.button_FinalizeDownload.IsEnabled = true;
		parentWindow.button_FinalizeDownload.Content = "Close finished";
		parentWindow.button_FinalizeDownload.Click += FinalizeDownload;
	}
	else if (downloadTask.state == "fail")
	{
		parentWindow.button_FinalizeDownload.IsEnabled = true;
		parentWindow.button_FinalizeDownload.Content = "Close unfinished";
		parentWindow.button_FinalizeDownload.Click += FinalizeDownload;
	}
}


private void ThisUnchecked(object sender, RoutedEventArgs e)
{
	parentWindow.button_EditImage.IsEnabled = false;
	parentWindow.button_EditImage.Click -= EditImage;

	parentWindow.button_FinalizeDownload.IsEnabled = false;
	parentWindow.button_FinalizeDownload.Content = "Close element";
	parentWindow.button_FinalizeDownload.Click -= FinalizeDownload;
}
```

  - MainWindow
```c#
public void SingleToggle(object sender)
{
	var toggledButton = sender as ToggleButton;
	foreach (var child in stackPanel_downloadElems.Children)
		if (child is DownloadElement downloadElement && downloadElement != toggledButton && downloadElement.IsChecked == true)
			downloadElement.IsChecked = false;
}
```

10. **Na cestě do finále**

- Od funkcionality, které bysme si přáli zbývá málo.
- Teď potřebujeme nějak zjistit, že je stahování hotové. Pro to budeme mít metody `OnFail` `OnFinish` ve třídě `DownloadElement`, které budou vizualně zobrazovat stav, který stahování dosáhlo.
- Ve třídě `DownloadTask` přibydou také dvě metody: `NotifyFail` a `NotifyFinish`, které zavolají funkce `OnFail` a `OnFinish`.
- Do metody Download se ke každému načtení hodnoty state přidá volání jedné z těchto metod.

 - Řešení:
  - DownloadTask
```c#
private void NotifyFail()
{
	foreach (var observer in observers)
		observer.OnFail();
}

private void NotifyFinish()
{
	foreach (var observer in observers)
		observer.OnFinish();
}
```
a
```c#
public async Task Download()
{
    // Exceptions checking
    HttpResponseMessage response;

    try
    {
        response = await downloadClient.GetAsync(downLink, HttpCompletionOption.ResponseHeadersRead);
    }
    catch
    {
        NotifyFail(); //new code
        state = "failed";
        return;
    }
    
    if (!response.IsSuccessStatusCode)
    {
        MessageBox.Show($"Invalid image link\nWeb response: {response.StatusCode}");
        NotifyFail();      //new code
        state = "failed";
        return;
    }

    if (!response.Content.Headers.ContentType.ToString().Contains("image"))
    {
        MessageBox.Show("Given url is not an image url.");
        NotifyFail();     //new code
        state = "failed";
        return;
    }

    // Declaring variables
    long totalImgSize = response.Content.Headers.ContentLength ?? -1;

    bool canReportState = false;

    if (totalImgSize != -1)
        canReportState = true;

    long totalBytesRead = 0;

    var buffer = new byte[this.chunkSize * updateMulitplier];
    int bytesRead;

    var chunkTimeStart = Stopwatch.StartNew();

    // Logic
    try
    {
        using (var contentStream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                chunkTimeStart.Restart();

                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                if (chunkSize != buffer.Length)
                    buffer = new byte[this.chunkSize * updateMulitplier];


                if (canReportState)
                    UpdateProgress((double)totalBytesRead / totalImgSize);

                await Task.Delay(int.Max(0, (1000 * updateMulitplier) - (int)chunkTimeStart.ElapsedMilliseconds));
            }

            state = "finished";
            NotifyFinish();    //new code
        }
    }

    catch
    {
        state = "failed";
        NotifyFail();    //new code
    }
        
}
```

  - DownloadElement
```c#
public void OnFail()
{
	Application.Current.Dispatcher.Invoke(() =>
	{
		BorderBrush = Brushes.Red;
		
	});
}

public void OnFinish()
{
	Application.Current.Dispatcher.Invoke(() =>
	{
		BorderBrush = Brushes.Green;
	});
}
```


11. **Konečná**

- Jsme již na konci a zbývá už jediné, hezky po sobě uklidit.
- Vytvoříme si metody `FinalizeDownload` a `EditImage`, o které byly check metody ochuzeny. 
- První metoda zavolá `uncheck` a zavolá novou metodu v `MainWindow` a druhá metoda vykreslí `MessageBox` s textem "Edit here".
- Nová metoda v `MainWindow` je `RemoveDownElement` a odebere ze `StackPanelu` svého potomka, který metodu zavolal.

 - Řešení:
  - DownloadElement
```c#
private void FinalizeDownload(object sender, RoutedEventArgs e)
{
	ThisUnchecked(this, e);
	parentWindow.RemoveDownElement(this);
}


private void EditImage(object sender, RoutedEventArgs e)
{
	// Your EditImage logic here
	MessageBox.Show("Edit here");
}
```

  - MainWindow
```c#
public void RemoveDownElement(object sender)
{
	stackPanel_downloadElems.Children.Remove((UIElement)sender);
}
```



### Zajímavé poznámky

- **Vytváření xml elementů v C#**
	- Ve WPF lze při běhu programu upravovat xml elementy, ale i přidávat nové. *(Součástí `System.Windows.Controls` modulu)* 

- **?? operátor**
	- '??' je null-koalesční operátor, který převede hodnotu null na nějakou default hodnotu. Umožňuje rychlý převod z typu s null hodnotu na typ bez null hodnoty. `set: int? num = null` `get: num = null` -> `set: int num = null ?? 0` `get: num = 0`

- **condition ? expression1 : expression2**
	- ternární operátor. Pokud je podmínka splněná, vrátí se `expression1`, pokud ne tak `expression2`.