
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