'use strict'
var n;
var g;
var h;
var z=0;
var schnitt=0;
var a=0;
var y=0;
var summe=0;
var noten=new Array(0);
function convert() {
	n  = document.getElementById('n').value;
	g = document.getElementById('g').value;
	noten.length++;
	noten[z]=new Array(2);
	noten[z][0]=n;
	noten[z][1]=g;
	z++;
	a=a+1*g;
	summe=summe+n*g;
	schnitt=summe/a;
	var ausgabe = document.getElementById('Schnitt');
	ausgabe.innerHTML ="Schnitt: "+ schnitt;
	var Note = document.getElementById('Note');
	var note="Note"+"__________"+"|";
	y=0;
	while(y<noten.length){
		note=note+noten[y][0]+"|";
		y++;
	}
	Note.innerHTML =note;
	var Gewichtung = document.getElementById('Gewicht');
	var gewicht="Gewichtung____|";
	y=0;
	while(y<noten.length){
		gewicht=gewicht+noten[y][1]+"|";
		y++;
	}
	Gewichtung.innerHTML =gewicht;
  //Eingabe.value = 1;
 }

var los  = document.getElementById('los');
los.addEventListener ('click', convert, true);