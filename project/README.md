# WPF Async






























## Informace mimo vhodn� zn�t pro hlavn� projekt

- **HttpClient**
	- T��da vytv��ej�c� propojen� s internetem.

- **Vytv��en� xml element� v C#**
	- Ve WPF lze p�i b�hu programu upravovat xml elementy, ale i p�id�vat nov�. *(Sou��st� `System.Windows.Controls` modulu)* 

- **?? oper�tor**
	- '??' je null-koales�n� oper�tor, kter� p�evede hodnotu null na n�jakou default hodnotu. Umo��uje rychl� p�evod z typu s null hodnotu na typ bez null hodnoty. `set: int? num = null` `get: num = null` -> `set: int num = null ?? 0` `get: num = 0`

- **condition ? expression1 : expression2**
	- tern�rn� oper�tor. Pokud je podm�nka spln�n�, vr�t� se `expression1`, pokud ne tak `expression2`.