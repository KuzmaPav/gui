# WPF Async






























## Informace mimo vhodné znát pro hlavní projekt

- **HttpClient**
	- Tøída vytváøející propojení s internetem.

- **Vytváøení xml elementù v C#**
	- Ve WPF lze pøi bìhu programu upravovat xml elementy, ale i pøidávat nové. *(Souèástí `System.Windows.Controls` modulu)* 

- **?? operátor**
	- '??' je null-koalesèní operátor, který pøevede hodnotu null na nìjakou default hodnotu. Umožòuje rychlý pøevod z typu s null hodnotu na typ bez null hodnoty. `set: int? num = null` `get: num = null` -> `set: int num = null ?? 0` `get: num = 0`

- **condition ? expression1 : expression2**
	- ternární operátor. Pokud je podmínka splnìná, vrátí se `expression1`, pokud ne tak `expression2`.