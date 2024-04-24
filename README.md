# gui


















## Informace mimo vhodné znát pro hlavní projekt
- **URi**
  - Je modul v C#, který převádí string URL adresy na objektovou formu. Nese v sobě i další informace.

- **HttpClient**
  - Třída vytvářející propojení s internetem. *(Používá instanci URI)*

- **Paralelizace**
  - Rozložení kódy mezi více dostupných jader, pro zlepšení rychlosti spousty výpočtů. 

- **Vytváření xml elementů v C#**
  - Ve WPF lze při běhu programu upravovat xml elementy, ale i přidávat nové. *(Součástí `System.Windows.Controls` modulu)* 

- **Více oknové aplikace**
  - Ve WPF lze vytvořit aplikace s více okny. Tyto okny se nemusí inicializovat při spuštění aplikace, ale i během běhu.

- **?? operátor**
  - '??' je null-koalesční operátor, který převede hodnotu null na nějakou default hodnotu. Umožňuje rychlý převod z typu s null hodnotu na typ bez null hodnoty. `set: int? num = null` `get: num = null` -> `set: int num = null ?? 0` `get: num = 0`