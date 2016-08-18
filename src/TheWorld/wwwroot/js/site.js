(function () {
        
    ////var ele = document.getElementById("username");
    ////ele.innerHTML = "Surya Kumari";
    ////var main = document.getElementById("main");
    ////main.onmouseenter = function () {
    ////    main.style = "background-color:gold";

    //var ele =$("#username");
    //ele.text("Surya Kumari");
    //var main = $("main");
    //main.on("mouseenter",function () {
    //    main.style = "background-color:gold";
    //});
    //main.on("mouseleave",function () {
    //    main.style = "";
    //});
    //var menuItems = $("ul.menu li a");
    //menuItems.on("click", function () {
    //    var me = $(this);
    //    alert(me.text());
    //});
    var $sidebarAndWrapper = $("#sidebar,#wrapper");
    var $icon = $("#sidebarToggle i.fa");
    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar");
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            // $(this).text("Show Sidebar");
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        }
        else {
            //$(this).text("Hide Sidebar");
            $icon.addClass("fa-angle-left");
            $icon.removeClass("fa-angle-right");
        }
    });
}());
