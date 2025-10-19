function pageLoaded()
{
    Prism.highlightAll();

    const latexElements = document.querySelectorAll('.formula');
    latexElements.forEach(function (element)
    {
        var latex = element.innerText;
        katex.render(latex, element, {
            throwOnError: false,
            output: "html",
            displayMode: true
        });
    });
}