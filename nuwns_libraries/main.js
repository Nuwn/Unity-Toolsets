function Main() {

    (function() {
        console.log("instanciated");
    })



    this.controller = function(callback){
        callback(this);
    }
}
var $nuwn =  new Main();
$nuwn.controller(function(core){})