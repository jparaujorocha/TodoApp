export function formatLocalDateTime(dateString) {
    if (!dateString) return '';
    try {
        const options = {
            year: 'numeric', month: 'short', day: 'numeric',
            hour: '2-digit', minute: '2-digit'
        };
        return new Date(dateString).toLocaleString(undefined, options);
    } catch (e) {
        console.error("Error formatting date:", dateString, e);
        return 'Invalid Date';
    }
}

export function getLocalDateTimeStringForInput(date = new Date()) {
    // Adjust for timezone offset to get local time in ISO-like format for input[type=datetime-local]
    const timezoneOffset = date.getTimezoneOffset() * 60000; // Offset in milliseconds
    const localISOTime = new Date(date.getTime() - timezoneOffset).toISOString().slice(0, 16);
    return localISOTime;
}