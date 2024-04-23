# C# - Asynchronní programování pomocí WPF-ASYNC
## Asynchronní programování v jazyce C#
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
```
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
