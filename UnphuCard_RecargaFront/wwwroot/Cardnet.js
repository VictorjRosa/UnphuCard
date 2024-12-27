
//async function cardnetTokenizar(cardNumber, expirationDate, cvv) {
//    try {
//        const response = await fetch("https://sandbox.cardnet.com.do/api/token", { // Reemplaza con el endpoint de tokenización real
//            method: "POST",
//            headers: {
//                "Content-Type": "application/json",
//                "Authorization": "Bearer TU_TOKEN_AQUI" // Coloca el token de autenticación si es necesario
//            },
//            body: JSON.stringify({
//                cardNumber: cardNumber.replace(/\s+/g, ""), // Limpia el número de tarjeta
//                expirationDate: expirationDate,
//                cvv: cvv
//            })
//        });

//        if (!response.ok) {
//            throw new Error("Error en la tokenización: " + response.statusText);
//        }

//        const data = await response.json();
//        return data.token; // Asegúrate de que el campo correcto sea el token en la respuesta
//    } catch (error) {
//        console.error("Error al tokenizar:", error);
//        throw error;
//    }
//}

