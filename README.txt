Elõkövetelmények
	PYthon 2.7+
	.NET Framework 4.5

Config
	A ..\pythonpath.config fájlban kell átírni a saját Python 2.7 telepítés útvonalat. 
	Mindenképp a pythonw.exe-t állítsd be és ne a python.exe-t.	

Használat
	A Run.vbs-el indítható a program
	A Train Neural Network a ResultPath-ban található tanító típusú fájlok (szám kiterjesztés, mely a jelöltek számát jelenti) alapján enged neurális hálókat tanítani. A hálók szintén a ResultPath-ba kerülnek.
	A Use Neural Network a ResultPath-ban található bemeneti (.noncond) és neurális háló (.network) kombinálásával enged neurális hálókat futtatni. Az eredmények (.result) szintén a ResultPath-ba kerülnek.
	A Generate Main Input-ban adhatóak meg a tanító és bemeneti fájlok generálásának paraméterei. Egyszerre 1-1 fájl generálható. Az elkészült fájlok a ResultPath-ba kerülnek.