
/**
 * 
 * 
 * @param {string} method GET returns array, Subdomain returns string, Directory returns path
 * @param {string} url
 * @returns 
 */
function get_url_params(method, url){
    if(method === "GET")
    {

    }
    else if(method === "subdomain")
    {

    }
    else if(method === "directory")
    {
        var first = url.split("//");
        var directories = first[1].split("/").splice(1);
        var array = [];
        for(i in directories)
        {
            if(directories[i] !== "")
            {
                array.push(directories[i]);
            }
        }
        return "/" + array.join("/") + "/";
    }
    else 
    {
        return null;
    }
}


/**
 * 
 * 
 * @param {object} obj1 
 * @param {object} obj2 
 * @returns 
 */
function merge_objects(obj1, obj2){
    var obj3 = {};
    for (var attrname in obj1) { obj3[attrname] = obj1[attrname]; }
    for (var attrname in obj2) { obj3[attrname] = obj2[attrname]; }
    return obj3;
}
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
/**
 *  
 * @params {string} what /direct/ories  
 * @params {object} params {TemplateUrl string, Selector array, Controller string}
 */
function $view(){
    this.routes = [];
    this.load_page = function (page, selector)
    {
        return new Promise(function(resolve, reject)
        {     
            var xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function() 
            {
                if (this.readyState == 4) 
                {
                    if (this.status === 200)
                    {
                        document.getElementById(selector).innerHTML = this.responseText;
                        resolve(200);
                    } 
                    else 
                    {
                        reject(Error(404));
                    }
                }
            };
            xhttp.open("GET", page, true);
            xhttp.send();
        });
    };
    this.load_js = function(js){
        function addJS(js){
            if(!document.getElementById('scripts')){
                var body = document.getElementsByTagName('body')[0];
                var newDOM = document.createElement('div');
                newDOM.setAttribute('id', 'scripts');
                body.appendChild(newDOM);
            }

            var body = document.getElementById('scripts'),
            script = document.createElement('script');
            script.src = js;
            body.appendChild(script);
        }
        function removeJS(){
            if(document.getElementById('scripts')){
                var DOM = document.getElementById('scripts');
                DOM.parentElement.removeChild(DOM);
            }
        }
        if(js == undefined){
            return;
        }
        removeJS();
        for(i in js){
            addJS(js[i]);
        }
    }
    this.when = function (what, params) 
    {  
        this.routes.push({what: what, params : params, active: false});
        this.init();
    }
    this.init = function () {
        var url = get_url_params("directory", window.location.href);
        var Params = "/";
        if(url.search("#") >= 0){
            var GetParams = url.split('#');
            if(GetParams[1] === "/"){
                Params = GetParams[1];
            } else {
                Params = GetParams[1].substring(0, GetParams[1].length - 1);
            }
        }   
        var error = true;
        
        for(i in this.routes){
            if(this.routes[i].what === Params){
                var j = 0;
                var data = this.routes[i];
                function load(){
                    if(j < data.params.TemplateUrl.length){
                        $view.load_page( data.params.TemplateUrl[j], data.params.Selector[j])
                        .then(function(){
                            if(j === data.params.TemplateUrl.length -1) 
                            {
                                $view.load_js(data.params.Controllers);
                            }
                            load();
                            j++;
                        }); 
                    }
                }
                if(!data.active){
                    load(this.routes);
                }
                data.active = true;
                error = false;
            } else {
                this.routes[i].active = false;
            }
        }
        if(error === true){
            this.routes.forEach(function(element) {
                if(element.what === "404"){
                    this.load_page( element.params.TemplateUrl[0], element.params.Selector[0]).then(  );
                }  
            }, this);
        }
    }   
} 
var $view = new $view;
window.onhashchange = function(){ $view.init(); }

/*








function loadView(string) {  
    this.string = string;  
    this.loader = document.getElementById("loader");
    this.load = function(){
        //start loader
        loader.style.display = "flex";
        //get the page with ajax
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function() {
            if (this.readyState == 4) {
                if (this.status === 200 || this.status === 0){
                    //display the template
                    document.getElementById("view").innerHTML = this.responseText;
                    runWhenURLMet();
                    //end loader
                    loader.style.display = "none";  
                } else {
                    //end loader
                    loader.style.display = "none";
                }
            }
        };
        xhttp.open("GET", "templates/"+string+".html", true);
        xhttp.send();  
    };
}

//when the document is loaded we check the hash if its refreshed, or if its a new instance we go to default template
document.addEventListener("deviceready", onDeviceReady, false);
function onDeviceReady() {
    var view;
    if(location.hash.slice(1)){
        view = new loadView(location.hash.slice(1).split("?")[0]);
        view.load(); 
    } else {
        view = new loadView("login");
        view.load();
    } 
    
}

// Adds an eventlistner on window to check whether the url changes with # 
window.addEventListener("hashchange", hashChange);
function hashChange() {
    var view;
    var loc = location.hash.slice(1);
    if(loc === ''){
        view = new loadView("login");
        view.load();
    } else if (loc === 'login') {
        view = new loadView("login");
        view.load();
    } else {
        view = new loadView(location.hash.slice(1).split("?")[0]);
        view.load();
    }
    
}

// things to do when a page is loaded
function runWhenURLMet(){
    url = location.hash.slice(1).split("?")[0];
    data = location.hash.slice(1).split("?");
    
    setActive(url);

    if (url === "home"){
        list("products", "homeul",true);
    }
    if (url === "login" || url === ""){
        document.getElementById("menu").style.display = "none";
        document.getElementById("returnBtn").style.display = "none";
    }
    if (url !== "login" && url !== ""){
        document.getElementById("menu").style.display = "flex";
        document.getElementById("returnBtn").style.display = "block";
    }
    if (url === "detail"){
        details(data[1]);
    }
}*/