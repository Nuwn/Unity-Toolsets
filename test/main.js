$view.when("/1", {
    TemplateUrl: ["views/view1.html"], 
    Selector: ["main"], 
    Controllers: ["js/test.js"] 
})
$view.when("/2", {
    TemplateUrl: ["views/view2.html"], 
    Selector: ["main"], 
    Controllers: [] 
})
$view.when("/", {
    TemplateUrl: ["views/view1.html"], 
    Selector: ["main"], 
    Controllers: [] 
})
$view.when("404", {
    TemplateUrl: ["views/404.html"], 
    Selector: ["main"], 
    Controllers: [] 
});
$view.when("/2/3", {
    TemplateUrl: ["views/nested/nestedtest.html"], 
    Selector: ["main2"], 
    Controllers: [] 
});