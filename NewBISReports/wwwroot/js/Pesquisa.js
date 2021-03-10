function pesquisar() {
    var btn = document.getElementById("pesquisa");
    document.getElementById("form").submit();
    btn.className = "btn btn - primary disabled btn - lg";
    btn.disabled = true;
    btn.innerHTML = "Pesquisando...";
}