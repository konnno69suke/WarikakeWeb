// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


    function resttext() {
        $('#members tr').each(function (i, row) {
            // payがonのメンバーを編集可にする
            var pch = $(row).find('td:nth-child(2) input[type="checkbox"]');
            var ptx = $(row).find('td:nth-child(3) input[type="number"]');
            if (pch.prop('checked')) {
                ptx.prop('disabled', false);
            } else {
                ptx.prop('disabled', true);
            }

            // repayがonのメンバーを編集可にする
            var rch = $(row).find('td:nth-child(6) input[type="checkbox"]');
            var rtx = $(row).find('td:nth-child(7) input[type="number"]');
            if (rch.prop('checked')) {
                rtx.prop('disabled', false);
            } else {
                rtx.prop('disabled', true);
            }
        });
        calccost();
    }


    function calccost() {
        // 支払額を取得
        var totalCost = $('#CostAmount').val();
    // 立替人数
    var payOn = 0;
    // 精算人数
    var repOn = 0;
    $('#members tr').each(function (i, row) {
        // payがonのメンバー数を取得
        $(row).find('td:nth-child(2) input[type="checkbox"]').each(function (j, input) {
            if ($(input).prop('checked')) {
                payOn++;
            }
        });
    // repayがonのメンバー数を取得
    $(row).find('td:nth-child(6) input[type="checkbox"]').each(function (k, input) {
                if ($(input).prop('checked')) {
        repOn++;
                }
            });
        });
    // 0除算の回避
    payOn = (payOn == 0 ? 1 : payOn);
    repOn = (repOn == 0 ? 1 : repOn);
    // 立替額
    var payQ = Math.trunc(totalCost / payOn);
    // 立替端数
    var payR = Math.trunc(totalCost % payOn);
    // 精算額
    var repQ = Math.trunc(totalCost / repOn);
    // 精算端数
    var repR = Math.trunc(totalCost % repOn);
    // 端数優先先
    var qid = $('#qid').val();
    $('#members tr').each(function (i, row) {
            // payがonのメンバーに値をセット
            var pui = $(row).find('td:nth-child(1) input[type="hidden"]');
    var pch = $(row).find('td:nth-child(2) input[type="checkbox"]');
    var ptx = $(row).find('td:nth-child(3) input[type="number"]');
    if (pch.prop('checked')) {
                if (pui.val() == qid) {
        ptx.val(payQ + payR);
                } else {
        ptx.val(payQ);
                }
            } else {
                if (pui.val() == qid) {
        ptx.val(payR);
                } else {
        ptx.val(0);
                }
            }

    // repayがonのメンバーに値をセット
    var rui = $(row).find('td:nth-child(5) input[type="hidden"]');
    var rch = $(row).find('td:nth-child(6) input[type="checkbox"]');
    var rtx = $(row).find('td:nth-child(7) input[type="number"]');
    if (rch.prop('checked')) {
                if (rui.val() == qid) {
        rtx.val(repQ + repR);
                } else {
        rtx.val(repQ);
                }
            } else {
                if (rui.val() == qid) {
        rtx.val(repR);
                } else {
        rtx.val(0);
                }
            }
        });
    }
    function setmonth() {
        if ($('#isAllMonth').prop('checked')) {
            var allFlg = true;
        } else {
            var allFlg = false;
        }
        $('#dateSubscribe_m1').prop('checked', allFlg);
        $('#dateSubscribe_m2').prop('checked', allFlg);
        $('#dateSubscribe_m3').prop('checked', allFlg);
        $('#dateSubscribe_m4').prop('checked', allFlg);
        $('#dateSubscribe_m5').prop('checked', allFlg);
        $('#dateSubscribe_m6').prop('checked', allFlg);
        $('#dateSubscribe_m7').prop('checked', allFlg);
        $('#dateSubscribe_m8').prop('checked', allFlg);
        $('#dateSubscribe_m9').prop('checked', allFlg);
        $('#dateSubscribe_m10').prop('checked', allFlg);
        $('#dateSubscribe_m11').prop('checked', allFlg);
        $('#dateSubscribe_m12').prop('checked', allFlg);
    }

    function setweekday() {
        if ("week" == $('input[name="weekOrDay"]:checked').val()) {
            var weekFlg = true;
        } else if ("day" == $('input[name="weekOrDay"]:checked').val()) {
            var weekFlg = false;
        } else { 
            return;
        }
        $('#dateSubscribe_r1').prop('disabled', !weekFlg);
        $('#dateSubscribe_r2').prop('disabled', !weekFlg);
        $('#dateSubscribe_r3').prop('disabled', !weekFlg);
        $('#dateSubscribe_r4').prop('disabled', !weekFlg);
        $('#dateSubscribe_r5').prop('disabled', !weekFlg);

        $('#dateSubscribe_w1').prop('disabled', !weekFlg);
        $('#dateSubscribe_w2').prop('disabled', !weekFlg);
        $('#dateSubscribe_w3').prop('disabled', !weekFlg);
        $('#dateSubscribe_w4').prop('disabled', !weekFlg);
        $('#dateSubscribe_w5').prop('disabled', !weekFlg);
        $('#dateSubscribe_w6').prop('disabled', !weekFlg);
        $('#dateSubscribe_w7').prop('disabled', !weekFlg);

        $('#dateSubscribe_d1').prop('disabled', weekFlg);
        $('#dateSubscribe_d2').prop('disabled', weekFlg);
        $('#dateSubscribe_d3').prop('disabled', weekFlg);
        $('#dateSubscribe_d4').prop('disabled', weekFlg);
        $('#dateSubscribe_d5').prop('disabled', weekFlg);
        $('#dateSubscribe_d6').prop('disabled', weekFlg);
        $('#dateSubscribe_d7').prop('disabled', weekFlg);
        $('#dateSubscribe_d8').prop('disabled', weekFlg);
        $('#dateSubscribe_d9').prop('disabled', weekFlg);
        $('#dateSubscribe_d10').prop('disabled', weekFlg);
        $('#dateSubscribe_d11').prop('disabled', weekFlg);
        $('#dateSubscribe_d12').prop('disabled', weekFlg);
        $('#dateSubscribe_d13').prop('disabled', weekFlg);
        $('#dateSubscribe_d14').prop('disabled', weekFlg);
        $('#dateSubscribe_d15').prop('disabled', weekFlg);
        $('#dateSubscribe_d16').prop('disabled', weekFlg);
        $('#dateSubscribe_d17').prop('disabled', weekFlg);
        $('#dateSubscribe_d18').prop('disabled', weekFlg);
        $('#dateSubscribe_d19').prop('disabled', weekFlg);
        $('#dateSubscribe_d20').prop('disabled', weekFlg);
        $('#dateSubscribe_d21').prop('disabled', weekFlg);
        $('#dateSubscribe_d22').prop('disabled', weekFlg);
        $('#dateSubscribe_d23').prop('disabled', weekFlg);
        $('#dateSubscribe_d24').prop('disabled', weekFlg);
        $('#dateSubscribe_d25').prop('disabled', weekFlg);
        $('#dateSubscribe_d26').prop('disabled', weekFlg);
        $('#dateSubscribe_d27').prop('disabled', weekFlg);
        $('#dateSubscribe_d28').prop('disabled', weekFlg);
        $('#dateSubscribe_d29').prop('disabled', weekFlg);
        $('#dateSubscribe_d30').prop('disabled', weekFlg);
        $('#dateSubscribe_d31').prop('disabled', weekFlg);
    }