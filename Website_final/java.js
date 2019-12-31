function Formel() {
  var n  = document.getElementById('n');
  alert("Die Summenformel des Moleküls bei " + n.value +" Vorgängen ist C"+ (n.value*2*18+21) +"H"+ (15+n.value*2*12) +"O"+ (4+3*2*n.value));
  Eingabe.value = 1;
 }

var los  = document.getElementById('los');
los.addEventListener ('click', Formel, true);