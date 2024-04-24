# C# - Asynchronní programování pomocí WPF-ASYNC
## Asynchronní programování v jazyce C#
**CZE Prezentace** https://www.canva.com/design/DAGDQIW3l9A/oUIUeN-_v6rqmi90P29Nrw/edit?utm_content=DAGDQIW3l9A&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton

**ENG Presentation** https://www.canva.com/design/DAGDQefAo5M/oInU8AKs4Um8092NbG9MNQ/edit?utm_content=DAGDQefAo5M&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton
### Asynchronní metody, async a await
**Asynchronie** umožňuje přesunout jednotlivé úlohy z hlavního vlákna do speciálních asynchronních metod a využívat vlákna úsporněji. Asynchronní metody se provádějí v samostatných vláknech. Po provedení dlouhé operace se však vlákno asynchronní metody vrátí do fondu vláken a použije se pro další úlohy. A když dlouhá operace dokončí své provádění, je pro asynchronní metodu opět přiděleno vlákno z fondu vláken a asynchronní metoda pokračuje ve své práci.

Klíčem k práci s asynchronními voláními v jazyce C# jsou dva operátory: **async** a **await**, jejichž cílem je zjednodušit psaní asynchronního kódu. Používají se společně k vytvoření asynchronní metody.

**Asynchronní metoda** má následující vlastnosti:
* Modifikátor async se používá v záhlaví metody
* Metoda obsahuje jeden nebo více výrazů await
* Jako návratový typ je použit jeden z následujících:
   * void
   * Task
   * Task<T>
   * ValueTask<T>

Asynchronní metoda, stejně jako normální metoda, může používat libovolný počet parametrů nebo žádné parametry. Asynchronní metoda však nemůže definovat parametry pomocí modifikátorů **out**, **ref** a **in**.

Za zmínku stojí také to, že slovo **async**, které je uvedeno v definici metody, z ní automaticky NEDĚLÁ metodu asynchronní. Pouze naznačuje, že metoda může obsahovat jeden nebo více příkazů Asynchronní metoda, stejně jako normální metoda, může používat libovolný počet parametrů nebo žádné parametry. Asynchronní metoda však nemůže definovat parametry pomocí modifikátorů out, ref a in.

Za zmínku stojí také to, že slovo asynchronní, které je uvedeno v definici metody, z ní automaticky NEDĚLÁ metodu asynchronní. Pouze naznačuje, že metoda může obsahovat jeden nebo více příkazů **await**.

Uvažujme nejjednodušší příklad definice a volání asynchronní metody:.
```C#
await PrintAsync();   // volání asynchronní metody
Console.WriteLine("Některé akce v metodě Main");
 
void Print()
{
    Thread.Sleep(3000);     // simulace nepřetržitého provozu
    Console.WriteLine("Hello WORLD AND STUDENTS");
}
 
// definice asynchronní metody
async Task PrintAsync()
{
    Console.WriteLine("Začátek metody PrintAsync"); // probíhá synchronně
    await Task.Run(() => Print());                // asynchronně
    Console.WriteLine("Konec metody PrintAsync");
}   
```

Zde je nejprve definována obvyklá metoda Print, která jednoduše vypíše nějaký řetězec na konzolu. Pro simulaci dlouhé operace se používá třísekundová prodleva pomocí metody Thread.Sleep(). Konvenčně je tedy Print nějaká metoda, která provádí nějakou dlouhou operaci. V reálné aplikaci to může být přístup k databázi nebo čtení a zápis souboru, ale pro zjednodušení pochopení se prostě vytiskne nějaký řetězec na konzoli.

Zde je také definována asynchronní metoda PrintAsync(). Je asynchronní, protože má ve své definici před návratovým typem modifikátor **async**, její návratový typ je Task a v těle metody je definován výraz **await**.

Je třeba poznamenat, že metoda PrintAsync explicitně nevrací žádný objekt Task, ale protože je v těle metody použit výraz **await**, lze jako návratový typ použít typ Task.

Podívejme se, jaký bude mít program konzolový výstup:
```
Zacatek metody PrintAsync
Hello WORLD AND STUDENTS
Konec metody PrintAsync
Nektere akce v metode Main
```
1. Spustí se program, přesněji metoda Main, ve které se zavolá asynchronní metoda PrintAsync.
2. Metoda PrintAsync se začne vykonávat synchronně až po výraz await.
3. Výraz await spustí asynchronní úlohu Task.Run((()=>Print()))).
4. Zatímco asynchronní úloha Task.Run((()=>Print())) běží (a může běžet poměrně dlouho), provádění kódu se vrací k volající metodě - tedy k metodě Main.
5. Jakmile asynchronní úloha dokončí své provádění (ve výše uvedeném případě vytiskla řádek po třech sekundách), pokračuje v běhu asynchronní metoda PrintAsync, která asynchronní úlohu volala.
6. Po dokončení metody PrintAsync pokračuje ve svém běhu metoda Main.

### Asynchronní metoda Main
Je třeba vzít v úvahu, že operátor **await** lze použít pouze v metodě, která má modifikátor **async**. A pokud operátor **await** použijeme v metodě Main, musí být metoda Main také definována jako asynchronní. To znamená, že předchozí příklad bude vlastně podobný tomu následujícímu:
```C#
class Program
{
    async static Task Main(string[] args)
    {
        await PrintAsync();   // volání asynchronní metody
        Console.WriteLine("Některé akce v metodě Main");
 
 
        void Print()
        {
            Thread.Sleep(3000);     // simulace nepřetržitého provozu
            Console.WriteLine("Hello WORLD AND STUDENTS");
        }
 
        // definice asynchronní metody
        async Task PrintAsync()
        {
            Console.WriteLine("Začátek metody PrintAsync"); // provedeno synchronně
            await Task.Run(() => Print());                // provedeno asynchronně
            Console.WriteLine("Konec metody PrintAsync");
        }
    }
}
```

### Zpoždění asynchronní operace a Task.Delay
V asynchronních metodách lze metodu **Task.Delay()** použít k zastavení metody na určitou dobu. Jako parametr přijímá počet milisekund jako hodnotu int nebo objekt TimeSpan, který určuje dobu zpoždění:
```C#
await PrintAsync();   // volání asynchronní metody
Console.WriteLine("Některé akce v metodě Main");
 
// definice asynchronní metody
async Task PrintAsync()
{
    await Task.Delay(3000);     // simulace nepřetržitého provozu
    // nebo tak nějak
    //await Task.Delay(TimeSpan.FromMilliseconds(3000));
    Console.WriteLine("Hello WORLD AND STUDENTS");
}   
```
Metoda Task.Delay je navíc sama o sobě asynchronní operací, takže je na ni aplikován operátor await.

### Výhody asynchronnosti
Výše uvedené příklady jsou zjednodušené a lze je jen stěží považovat za ilustrativní. Uvažujme jiný příklad:
```C#
PrintName("Tom");
PrintName("Bob");
PrintName("Sam");
 
void PrintName(string name)
{
    Thread.Sleep(3000);     // 
    Console.WriteLine(name);
}
```
Tento kód je synchronní a provede postupně tři volání metody PrintName. Vzhledem k tomu, že metoda má třísekundové zpoždění, které simuluje dlouhou operaci, bude celkové provedení programu trvat nejméně 9 sekund. Protože každé další volání metody PrintName bude čekat, dokud nebude předchozí volání dokončeno.

Změňme synchronní metodu PrintName na asynchronní:
```C#
await PrintNameAsync("Tom");
await PrintNameAsync("Bob");
await PrintNameAsync("Sam");
 
// definice asynchronní metody
async Task PrintNameAsync(string name)
{
    await Task.Delay(3000);     // simulace nepřetržitého provozu
    Console.WriteLine(name);
}
```
Namísto metody PrintName je nyní metoda PrintNameAsync volána třikrát. Pro simulaci dlouhé doby běhu je metoda odložena o 3 sekundy voláním Task.Delay(3000). A protože každé volání metody využívá operátor await, který zastaví její provádění, dokud není asynchronní metoda dokončena, bude celkové provádění programu opět trvat nejméně 9 sekund. Nicméně nyní provádění asynchronních operací neblokuje hlavní vlákno.

Nyní program optimalizujme:
```C#
var tomTask = PrintNameAsync("Tom");
var bobTask = PrintNameAsync("Bob");
var samTask = PrintNameAsync("Sam");
 
await tomTask;
await bobTask;
await samTask;
// definice asynchronní metody
async Task PrintNameAsync(string name)
{
    await Task.Delay(3000);     // simulace nepřetržitého provozu
    Console.WriteLine(name);
}
```
V tomto případě se úlohy skutečně spouštějí při definici. A operátor await se používá pouze tehdy, když potřebujeme počkat na dokončení asynchronních operací - tedy na konci programu. A v tomto případě bude celkové provedení programu trvat ne méně než 3 sekundy, ale mnohem méně než 9 sekund.

## Vrátit výsledek z asynchronní metody
Jako návratový typ v asynchronní metodě musí být použit typ **void**, **Task**, **Task<T>** nebo **ValueTask<T>**.

### void
Pokud je použito klíčové slovo void, asynchronní metoda nic nevrací:
```C#
PrintAsync("Hello World");
PrintAsync("Hello STUDENTS");
 
Console.WriteLine("Main End");
await Task.Delay(3000); // čekání na dokončení úkolů
 
// definice asynchronní metody
async void PrintAsync(string message)
{
    await Task.Delay(1000);     // simulace nepřetržitého provozu
    Console.WriteLine(message);
}   
```
Asynchronním metodám void je však třeba se vyhnout a používat je pouze v případech, kdy podobné metody představují jediný možný způsob, jak definovat asynchronní metodu. Především na takové metody nemůžeme použít operátor await. Také proto, že výjimky v takových metodách se obtížně ošetřují, protože je nelze zachytit mimo metodu. Kromě toho se takové void metody obtížně testují.

Přesto existují situace, kdy se bez takových metod neobejdeme - například při zpracování událostí:
```C#
Account account = new Account();
account.Added += PrintAsync;
 
account.Put(500);
 
await Task.Delay(2000); // čeká na dokončení
 
// definice asynchronní metody
async void PrintAsync(object? obj, string message)
{
    await Task.Delay(1000);     // simulace nepřetržitého provozu
    Console.WriteLine(message);
}
 
class Account
{
    int sum = 0;
    public event EventHandler<string>? Added;
    public void Put(int sum)
    {
        this.sum += sum;
        Added?.Invoke(this, $"Na účet byl připsán {sum} $");
    }
}
```
V tomto případě je událost Přidáno ve třídě Účet reprezentována delegátem EventHandler, který má typ void. Proto lze pro tuto událost definovat pouze metodu obsluhy s typem void.

### Task
Vrátí objekt typu Task:
```C#
await PrintAsync("Hello WORLD AND STUDENTS");
 
// definice asynchronní metody
async Task PrintAsync(string message)
{
    await Task.Delay(1000);     // simulace nepřetržitého provozu
    Console.WriteLine(message);
}
```
Zde metoda PrintAsync formálně nepoužívá k vrácení výsledku operátor return. Pokud je však v asynchronní metodě v příkazu await provedena asynchronní operace, můžeme z metody vrátit objekt Task.

Chceme-li počkat na dokončení asynchronní úlohy, můžeme použít operátor **await**. A není nutné jej používat přímo při volání úlohy. Lze jej použít pouze tam, kde potřebujeme zaručit, že dostaneme výsledek úlohy, nebo se ujistit, že úloha byla dokončena.
```C#
var task = PrintAsync("Hello WORLD AND STUDENTS"); // spuštění úlohy
Console.WriteLine("Main Works");
 
await task; // počkat na dokončení úkolu
 
// definice asynchronní metody
async Task PrintAsync(string message)
{
    await Task.Delay(0);
    Console.WriteLine(message);
}
```

### Task < T >
Metoda může vracet nějakou hodnotu. Pak je vracená hodnota zabalena do objektu Task a vracený typ je Task<T>:
```c#
int n1 = await SquareAsync(5);
int n2 = await SquareAsync(6);
Console.WriteLine($"n1={n1}  n2={n2}"); // n1=25  n2=36
 
async Task<int> SquareAsync(int n)
{
    await Task.Delay(0);
    return n * n;
}
```
V tomto případě metoda Square vrací hodnotu typu int - čtverec čísla. Vrácený typ je tedy v tomto případě typu Task<int>.

Pro získání výsledku asynchronní metody použijeme při volání SquareAsync operátor await:
```
int n1 = await SquareAsync(5);
```
Podobným způsobem lze získat i další typy dat:
```C#
Person person = await GetPersonAsync("Tom");
Console.WriteLine(person.Name); // Tom
// definice asynchronní metody
async Task<Person> GetPersonAsync(string name)
{
    await Task.Delay(0);
    return new Person(name);
}
record class Person(string Name);
```

### ValueTask < T >
Použití typu ValueTask<T> je velmi podobné použití typu Task<T> až na některé rozdíly v práci s pamětí, protože ValueTask je struktura, která obsahuje více polí. Proto použití ValueTask místo Task vede ke kopírování většího množství dat a v důsledku toho vytváří určitou dodatečnou režii.

Výhodou ValueTask oproti Task je, že se tento typ vyhne dodatečné alokaci paměti v čipu. Někdy například potřebujete synchronně vrátit nějakou hodnotu. Vezměme si tedy následující příklad:
```C#
var result = await AddAsync(4, 5);
Console.WriteLine(result);
 
Task<int> AddAsync(int a, int b)
{
    return Task.FromResult(a + b);
}
```
Zde metoda AddAsync synchronně vrací nějakou hodnotu - v tomto případě součet dvou čísel. Pomocí statické metody Task.FromResult můžete synchronně vrátit nějakou hodnotu. Použití typu Task však povede k alokaci další úlohy s přidruženou alokací paměti v hipu. ValueTask tento problém řeší:
```C#
var result = await AddAsync(4, 5);
Console.WriteLine(result);
 
ValueTask<int> AddAsync(int a, int b)
{
    return new ValueTask<int>(a + b);
}
```
V tomto případě nebude vytvořen další objekt úlohy, a proto nebude alokována žádná další paměť. Proto se ValueTask obvykle používá v případech, kdy je výsledek asynchronní operace již k dispozici.
