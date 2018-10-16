
function calcular_Printed() {
    importe_total = 0;
    var error = 0;
    $(".print").each(
        function (index, value) {

            var input = $(this);
            var print = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numPrint = parseInt(print);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numPrint > numPO) {
                input.css('border', '2px solid #e03f3f');
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }

        }
    );

    if (error !== 0) {
        $('#printshop').prop("disabled", true);
    } else {
        $('#printshop').prop("disabled", false);
    }


    $("#totalP").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    calcular_Restantes();
}





function calcular_MisPrint() {
    importe_total = 0;
    $(".mp").each(
        function (index, value) {
            importe_total = importe_total + eval($(this).val());
        }
    );
    $("#totalM").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    calcular_Restantes();
}

function calcular_Defect() {
    importe_total = 0;
    $(".def").each(
        function (index, value) {
            importe_total = importe_total + eval($(this).val());
        }
    );
    $("#totalD").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    calcular_Restantes();
}

function calcular_TotalG() {
    importe_total = 0;
    var print = parseInt($("#totalP").val());
    var misPrint = parseInt($("#totalM").val());
    var defecto = parseInt($("#totalD").val());
    importe_total = print + misPrint + defecto;
    $("#totalF").val(parseInt(importe_total));

}



function CalcularTotalGeneral() {
    var sum = 0;
    $(".total").each(function () {
        sum += parseFloat($(this).text());
    });
    $('#sum').text(sum);
}
function CalcularTotal() {
    var sumQ = [];
    var nColumnas = $("#tablePrint tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFal").eq(i - 1).val(faltante);


        });

    }


}

function CalcularTotalBatch() {
    var sumQ = [];
    var nColumnas = $("#tablePrint tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFalt").eq(i - 1).val(valor);


        });

    }


}

function calcular_Restantes() {
    var error = 0;
    $(".totalFal").each(
        function (index, value) {
            var input = $(this);
            var print = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numPrint = parseInt(print);
            var valores;


            if (numPO >= numPrint) {
                input.css('background-color', '#f97878');

            }

            if (numPrint === 0) {
                input.css('background-color', '#eee');
            }

        }
    );
}