/* STARTANDO OS COMPONENTES DEPENDENTES DE JS EM 
 * CARDPERS.CSHTML
 * ------------------------------------------------*/
$(document).ready(function () {
    //Iniciando os selects2 da página CadPers
    $('#selectInterprise').select2({
        allowClear: true,
        placeholder: "Selecione Uma Empresa",
    })
    $('#selectUnity').select2({
        allowClear: true,
        placeholder: "Selecione Uma Unidade",
    })
    $('#selectCountry').select2({
        allowClear: true,
        placeholder: "Selecione Um País",
    })
    $('#selectState').select2({
        allowClear: true,
        placeholder: "Selecione Um Estado",
    })
    $('#selectGender').select2({
        allowClear: true,
        placeholder: "Selecione Um Gênero",
    })
    $('#autoPerfil').select2({
        allowClear: true,
        theme: "classic",
        placeholder: "Selecione as Autorizações",
    })
    $('#cmdPerfil').select2({
        allowClear: true,
        placeholder: "Selecione Um Perfil",
    })


    $.fn.datepicker.dates['pt-br'] = {
        days: ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"],
        daysShort: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"],
        daysMin: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"],
        months: ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"],
        monthsShort: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
        today: "Hoje",
        clear: "Limpar",
        format: "dd/mm/yyyy",
        titleFormat: "MM yyyy", /* Leverages same syntax as 'format' */
        weekStart: 0
    }

    //Inicializa o datepicker de data de nascimento .input-group.date.birthDay
    $('#datepickerBirthDay').datepicker({
        language: "pt-br",
        format: "dd/mm/yyyy",
        autoclose: "true",
        startDate: "01/01/1900",
    })

    //Recuperando a data do dia
    let date = new Date()

    //configurações para os datepickers de autorização
    $('#datapickerFrom').datepicker({
        language: "pt-br",
        autoclose: "true",
        todayHighlight: true,
        startDate: `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`,
    })
    $('#datapickerTo').datepicker({
        language: "pt-br",
        autoclose: "true",
        todayHighlight: true,
        startDate: `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`,
    })


    if ($('#datatable-column-Autorizacao').length > 0) {
        $('#datatable-column-Autorizacao').dataTable({
            sDom: "RC" +
                "t" +
                "<'row'<'col-sm-6'i><'col-sm-6'p>>",
            colVis: {
                buttonText: 'Show / Hide Columns',
                restore: "Restore",
                showAll: "Show all"
            },
        });
    }

    $.ajax({
        url: window.SerializaJsonGetPerfilP,
        type: "GET",
        dataType: "JSON",
        data: { profileid: $("#cmdPerfil").val() }, //id of the language
        traditional: true,
        success: function (result) {
            $('#autoPerfil').empty();
            $.each(result, function (i, item) {
                $('#autoPerfil').append($('<option value="' + item["shortname"] + '"> ' + item["shortname"] + ' </option>'));
            });
            $('#autoPerfil').bootstrapDualListbox('refresh', true);
        },
        error: function () {
            alert("Error ao carregar o Perfil.");
        }
    });

    //Função para evitar duplicação de autorizações de mesmo valor
    $('#autoPerfil').on('change', () => {
        //Variável que guardará os valores de altorizações multiplas selecionados
        let valueMultiple = $('.select2-search-choice div')
        //Encontrando duplicação de seleção na hora da seleção
        for (let i in valueMultiple) {
            if (valueMultiple[i].innerHTML == undefined)
                break
            for (let j in valueMultiple) {
                if (valueMultiple[j].innerHTML == undefined)
                    break
                if (valueMultiple[j].innerHTML == valueMultiple[i].innerHTML && i != j) {
                    let valueDuplicated = valueMultiple[j]
                    $(valueDuplicated).parent().remove()
                    return false
                } 
            }
        }
    })

    //Função para preenchimento automático das autorizações
    $('#cmdPerfil').on('change', () => {
        //let ProfileId = {
        //    profileid: $("#cmdPerfil").val()
        //}
        //let ProfileIdJson = JSON.stringify(ProfileId)
        let profileId = $("#cmdPerfil").val()
        $.ajax({
            url: window.SerializaJsonGetPerfilP,
            type: "GET",
            dataType: "JSON",
            data: { profileid: $("#cmdPerfil").val() }, //id of the language
            traditional: true,
            success: function (result) {
                $('#autoPerfil').empty();
                $.each(result, function (i, item) {
                    $('#autoPerfil').append($('<option value="' + item["shortname"] + '"> ' + item["shortname"] + ' </option>'));
                });
                $('#autoPerfil').bootstrapDualListbox('refresh', true);
                let val = $('#autoPerfil option')
                let valArray = Array()
                for (let i in val) {
                    //if (val[i].value === undefined)
                    //    break
                    valArray.push(val[i].value)
                }
                $('#autoPerfil').select2('val', valArray)
                $('#autoPerfil').trigger('change')
            },
            error: function () {
                alert("Error ao carregar o Perfil.");
            }
        });
    }) 
})



/* FUNÇÃO PARA LIMPAR TODO SELECT DE AUTORIZAÇÕES
 * ------------------------------------------------*/
function clearAllAutoPerfil() {
    $('#autoPerfil').val(null).trigger('change')
}





/* Alterando datepicker "Até" após a seleção de "De"
 * ------------------------------------------------*/
//$('#datapickerFrom').on('changeDate', () => {
//    //Recuperando data selecionada em "De"
//    let fromDate = $('#datapickerFrom').datepicker('getDate')

//    //Limpando datapicker "Até"
//    $('#datapickerTo').datepicker('update', '')

//    //Setando Default
//    $('#datapickerTo').datepicker('remove')

//    //Aplicando nova startDate
//    $('#datapickerTo').datepicker({
//        language: "pt-br",
//        autoclose: "true",
//        todayHighlight: true,
//        startDate: `${fromDate.getDate()}/${fromDate.getMonth() + 1}/${fromDate.getFullYear()}`,
//    })
//})



function formatar(mascara, documento) {
    var i = documento.value.length;
    var saida = mascara.substring(0, 1);
    var texto = mascara.substring(i)

    if (texto.substring(0, 1) != saida) {
        documento.value += texto.substring(0, 1);
    }

}
function excluirCartao(table) {
    var tabelaCartao = document.getElementById("tabelaCartao");
    var num = tabelaCartao.rows[table.rowIndex].cells[2].innerHTML;
    var sc = tabelaCartao.rows[table.rowIndex].cells[1].innerHTML;
    document.getElementById("Valor").value = num;
    document.getElementById("ValorAuxiliar").value = sc;
    $("#mdExcluirCartao").modal();
}
function excluirAuto(table) {
    var tabelaAuto = document.getElementById("tabelaAutorizacao");
    var name = tabelaAuto.rows[table.rowIndex].cells[1].innerHTML;
    document.getElementById("Valor").value = name;
    $("#mdExcluirAutorizacao").modal();
}
