function ticketPriBgColor() {
    let priority = document.getElementById("ticketPriorityText").innerText
    let ticketPri = "";
    if (priority == "Urgent") {
        ticketPri = "bg-danger";
    }
    else if (priority == "High") {
        ticketPri = "bg-warning";
    }
    else if (priority == "Medium") {
        ticketPri = "bg-success";
    }
    else if (priority == "Low") {
        ticketPri = "bg-primary";
    }
    document.getElementById("ticketPriorityBox").classList.toggle(ticketPri)
}
function ticketStatusBgColor() {
    let status = document.getElementById("ticketStatusText").innerText
    let ticketStatus = "";
    if (status == "Unassigned") {
        ticketStatus = "bg-danger";
    }
    else if (status == "Testing") {
        ticketStatus = "bg-warning";
    }
    else if (status == "Resolved") {
        ticketStatus = "bg-success";
    }
    else if (status == "Development") {
        ticketStatus = "bg-info";
    }
    else if (status == "New") {
        ticketStatus = "bg-primary"
    }
    document.getElementById("ticketStatusBox").classList.toggle(ticketStatus)
}
function ticketTypeBgColor() {
    let type = document.getElementById("ticketTypeText").innerText
    let ticketType = "";
    if (type == "Runtime") {
        ticketType = "bg-danger";
    }
    else if (type == "Maintenance") {
        ticketType = "bg-warning";
    }
    else if (type == "New Development") {
        ticketType = "bg-success";
    }
    else if (type == "UI") {
        ticketType = "bg-info";
    }
    document.getElementById("ticketTypeBox").classList.toggle(ticketType)
}
