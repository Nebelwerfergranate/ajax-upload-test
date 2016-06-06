
var first_step = document.getElementById('step_1');
var second_step = document.getElementById('step_2');
var third_step = document.getElementById('step_3');
var fourth_step = document.getElementById('step_4');


var drop_field = document.getElementById('js_drop_field');
var load_field = document.getElementById('js_load_field');
var list_photo = document.getElementById('js_list_photo');
var big_photo = document.getElementById('js_big_photo');

first_step.onclick = function() {
	drop_field.style.display = "block";
	load_field.style.display = "none";
	list_photo.style.display = "none";
	big_photo.style.display = "none";
}
second_step.onclick = function() {
	drop_field.style.display = "none";
	load_field.style.display = "block";
	list_photo.style.display = "none";
	big_photo.style.display = "none";
}
third_step.onclick = function() {
	drop_field.style.display = "block";
	load_field.style.display = "none";
	list_photo.style.display = "block";
	big_photo.style.display = "none";
}
fourth_step.onclick = function() {
	drop_field.style.display = "block";
	load_field.style.display = "none";
	list_photo.style.display = "block";
	big_photo.style.display = "block";
}