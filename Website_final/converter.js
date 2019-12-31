'use strict'
var n;
var se;
var ziffer;
var zahl;
var z;
var dez=0;
var aus;
var sa;
var a=0;
function convert() {
	n  = document.getElementById('n').value;
	se = document.getElementById('se').value;
	sa = document.getElementById('sa').value;
	ziffer=0;
	zahl=n;
	z=0;
	var ziffern=[0];
	while(zahl>0){
		ziffer = zahl%10;
		ziffern[z]=ziffer;
		zahl = (zahl-(zahl%10)) / 10;
		z=z+1;
		ziffern.length=ziffern.length+1;
	}
	ziffern.length--;
	//alert(ziffern);
	z=0;
	dez=0;
	while(z<ziffern.length){
		dez=dez+Math.pow(se, z)*ziffern[z];
		z++;
	}
	var ausgabe = document.getElementById('ausgabe');
	ausgabe.innerHTML = "Die Zahl "+ n +" im "+ se +"er- System ist im Dezimalsystem "+ dez +".";
	aus=0;
	aus="";
	while(dez>0){
		a=dez%sa
		if(a<10){
			aus=a + aus;
		}else{
			aus="|"+ a +"|";
		}
		dez=(dez-(dez%sa))/sa;
	}
	var ausgabez = document.getElementById('ausgabez');
	ausgabez.innerHTML = "Die Zahl "+ n +" im "+ se +"er- System ist im "+ sa +"er-System "+ aus +".";
  //Eingabe.value = 1;
 }

var los  = document.getElementById('los');
los.addEventListener ('click', convert, true);