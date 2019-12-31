function animate(){
x=x+1;
document.getElementById("ep1").style.width=x*5.81 +"px";
document.getElementById("ep1").style.height=x*7.82 +"px";
document.getElementById("ep1").style.marginBottom=391-x*3.91 +"px";
document.getElementById("ep1").style.marginTop=391-x*3.91 +"px";
document.getElementById("ep2").style.width=x*6.98 +"px";
document.getElementById("ep2").style.height=x*6.24 +"px";
document.getElementById("ep2").style.marginBottom=312-x*3.12 +"px";
document.getElementById("ep2").style.marginTop=312-x*3.12 +"px";
}
var z=1;
var x=0;
function init(){
	animate();
	while(z<100){
	setTimeout(animate, z*10+300);
	z++
	}
}
window.onload= init;